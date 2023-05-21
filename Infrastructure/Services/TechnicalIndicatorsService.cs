using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GptFinance.Infrastructure.Services
{
    public class TechnicalIndicatorsService : ITechnicalIndicatorsService
    {
        private readonly AppDbContext _context;
        private readonly IEodDataRepository _eodDataRepository;

        public TechnicalIndicatorsService(AppDbContext context, IEodDataRepository eodDataRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eodDataRepository = eodDataRepository ?? throw new ArgumentNullException(nameof(eodDataRepository));
        }

        public enum ImputationMethod
        {
            Mean,
            Median,
            Mode,
            LastObservationCarriedForward,
            LinearInterpolation
        }

        public Dictionary<DateTime, decimal> ImputeMissingData(Dictionary<DateTime, decimal?> priceData, ImputationMethod method)
        {
            var imputedData = new Dictionary<DateTime, decimal>();

            decimal? lastObservation = null;

            foreach (var dataPoint in priceData)
            {
                if (dataPoint.Value.HasValue)
                {
                    imputedData[dataPoint.Key] = dataPoint.Value.Value;
                    lastObservation = dataPoint.Value.Value;
                }
                else
                {
                    decimal imputedValue = 0;

                    switch (method)
                    {
                        case ImputationMethod.Mean:
                            imputedValue = priceData.Values.Where(v => v.HasValue).Average(v => v.Value);
                            break;
                        case ImputationMethod.Median:
                            imputedValue = Median(priceData.Values.Where(v => v.HasValue).Select(v => v.Value).ToList());
                            break;
                        case ImputationMethod.Mode:
                            imputedValue = priceData.Values.Where(v => v.HasValue).GroupBy(v => v).OrderByDescending(g => g.Count()).First().Key.Value;
                            break;
                        case ImputationMethod.LastObservationCarriedForward:
                            if (lastObservation.HasValue)
                            {
                                imputedValue = lastObservation.Value;
                            }
                            else
                            {
                                throw new InvalidOperationException("Cannot use LastObservationCarriedForward method for the first data point.");
                            }
                            break;
                    }

                    imputedData[dataPoint.Key] = imputedValue;
                }
            }

            return imputedData;
        }

        public decimal Median(List<decimal> values)
        {
            int numberCount = values.Count();
            int halfIndex = values.Count() / 2;
            var sortedNumbers = values.OrderBy(n => n);
            decimal median;
            if ((numberCount % 2) == 0)
            {
                median = ((sortedNumbers.ElementAt(halfIndex - 1) +
                           sortedNumbers.ElementAt(halfIndex)) / 2);
            }
            else
            {
                median = sortedNumbers.ElementAt(halfIndex);
            }

            return median;
        }



        public async Task CalculateAndStoreEma(int period, Company company)
        {
            var closingPrices = company.EodData.OrderBy(e => e.Date).ToDictionary(o => o.Date, o=> o.Close);
            var imputedClosingPrices = ImputeMissingData(closingPrices, ImputationMethod.LastObservationCarriedForward);
            var data = CalculateEMA(imputedClosingPrices, period);

            var entities = data.Select(d => new EmaData
            {
                CompanyId = company.Id,
                Date = d.Key,
                Period = period,
                Value = d.Value
            });

            _context.EmaData.AddRange(entities);

            await _context.SaveChangesAsync();
        }

        public async Task CalculateAndStoreEmaFan(int[] periods, ICollection<Company> companies)
        {
            foreach (var company in companies)
            {
                var eods = await _eodDataRepository.GetQuotesByCompanyId(company.Id);
                var closingPrices = eods.OrderBy(e => e.Date).ToDictionary(o => o.Date, o=> o.Close);
                var imputedClosingPrices = ImputeMissingData(closingPrices, ImputationMethod.LastObservationCarriedForward);

                foreach (var period in periods)
                {
                    var data = CalculateEMA(imputedClosingPrices, period);

                    var entities = data.Select(d => new EmaData
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = company.Id,
                        Date = d.Key,
                        Period = period,
                        Value = d.Value
                    });
                    _context.EmaData.AddRange(entities);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CalculateAndStoreMacd(int shortPeriod, int longPeriod, int signalPeriod, Company company)
        {
            var priceData = company.EodData.ToDictionary(o => o.Date, o => o.Close);
            var imputedClosingPrices = ImputeMissingData(priceData, ImputationMethod.LastObservationCarriedForward);
            var macdData = CalculateMACD(imputedClosingPrices, shortPeriod, longPeriod, signalPeriod);

            var keys = macdData.Histogram.Keys.Union(macdData.MACDLine.Keys).Union(macdData.SignalLine.Keys).Distinct().OrderBy(o => o.Date);
            var entities = new List<MacdData>();
            foreach (var date in keys)
            {
                var dataPoint = new MacdData
                {
                    CompanyId = company.Id,
                    Date = date,
                    ShortPeriod = shortPeriod,
                    LongPeriod = longPeriod,
                    SignalPeriod = signalPeriod,
                    MacdValue = macdData.MACDLine[date],
                    SignalValue = macdData.SignalLine[date],
                    HistogramValue = macdData.Histogram[date]
                };
                entities.Add(dataPoint);
            }
            _context.MacdData.AddRange(entities);

            await _context.SaveChangesAsync();
        }

        public async Task CalculateAndStoreMacdOnAllCompanies(int shortPeriod, int longPeriod, int signalPeriod, ICollection<Company> companies)
        {
            foreach (var company in companies)
            {
               await CalculateAndStoreMacd(shortPeriod, longPeriod, signalPeriod, company);
            }
        }


        /// <summary>
        /// Please note that this code does not handle missing dates in the input data,
        /// and it assumes that the input data is sorted in ascending order by date.
        /// If your data does not meet these criteria, you may need to preprocess it
        /// before passing it to this function.
        /// </summary>
        /// <param name="priceData"></param>
        /// <param name="shortPeriod"></param>
        /// <param name="longPeriod"></param>
        /// <param name="signalPeriod"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private (Dictionary<DateTime, decimal> MACDLine, Dictionary<DateTime, decimal> SignalLine, Dictionary<DateTime, decimal> Histogram) CalculateMACD(Dictionary<DateTime, decimal> priceData, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            // Check the data
            if (priceData.Count < longPeriod)
                throw new ArgumentException("Insufficient data to calculate MACD.");
            if (priceData.First().Key > priceData.Last().Key)
                throw new ArgumentException("Price data must be sorted in ascending order by date.");

            var macdLine = new Dictionary<DateTime, decimal>();
            var signalLine = new Dictionary<DateTime, decimal>();
            var histogram = new Dictionary<DateTime, decimal>();

            // Calculate EMA for short period and long period
            var shortEma = CalculateEMA(priceData, shortPeriod);
            var longEma = CalculateEMA(priceData, longPeriod);

            // Calculate MACD Line: (12-day EMA - 26-day EMA)
            foreach (var date in priceData.Keys)
            {
                if (shortEma.ContainsKey(date) && longEma.ContainsKey(date))
                {
                    macdLine[date] = shortEma[date] - longEma[date];
                }
            }

            // Calculate Signal Line: 9-day EMA of MACD Line
            signalLine = CalculateEMA(macdLine, signalPeriod);

            // Calculate MACD Histogram: MACD Line – Signal Line
            foreach (var date in macdLine.Keys)
            {
                if (signalLine.ContainsKey(date))
                {
                    histogram[date] = macdLine[date] - signalLine[date];
                }
            }

            return (macdLine, signalLine, histogram);
        }

        /// <summary>
        /// Please note that this code does not handle missing dates in the input data,
        /// and it assumes that the input data is sorted in ascending order by date.
        /// If your data does not meet these criteria, you may need to preprocess it
        /// before passing it to this function.
        /// </summary>
        /// <param name="priceData"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private Dictionary<DateTime, decimal> CalculateEMA(Dictionary<DateTime, decimal> priceData, int period)
        {
            if (priceData.First().Key > priceData.Last().Key)
                throw new ArgumentException("Price data must be sorted in ascending order by date.");

            var ema = new Dictionary<DateTime, decimal>();
            decimal multiplier = 2.0M / (period + 1);
            decimal emaPrev = 0;

            for (int i = 0; i < priceData.Count; i++)
            {
                if (i == 0)
                {
                    emaPrev = priceData.ElementAt(i).Value; // for the very first data point, SMA and EMA are same
                }
                else
                {
                    decimal close = priceData.ElementAt(i).Value;
                    emaPrev = (close - emaPrev) * multiplier + emaPrev; // EMA formula
                }
                ema[priceData.ElementAt(i).Key] = emaPrev;
            }

            return ema;
        }

        private decimal CalculateRSI(IEnumerable<decimal> closingPrices, int period)
        {
            var enumerable = closingPrices as decimal[] ?? closingPrices.ToArray();
            var gains = new List<decimal>();
            var losses = new List<decimal>();

            for (int i = 1; i < enumerable.Count(); i++)
            {
                var difference = enumerable.ElementAt(i) - enumerable.ElementAt(i - 1);
                if (difference > 0)
                    gains.Add(difference);
                else
                    losses.Add(Math.Abs(difference));
            }

            var averageGain = gains.Average();
            var averageLoss = losses.Average();

            var relativeStrength = averageGain / averageLoss;
            var rsi = 100 - (100 / (1 + relativeStrength));

            return rsi;
        }

        private decimal CalculateStochasticOscillator(IEnumerable<decimal> closingPrices, int period)
        {
            var enumerable = closingPrices as decimal[] ?? closingPrices.ToArray();
            var lowest = enumerable.Take(period).Min();
            var highest = enumerable.Take(period).Max();
            var current = enumerable.Last();

            var stochasticOscillator = (current - lowest) / (highest - lowest) * 100;

            return stochasticOscillator;
        }

        private decimal CalculateStochasticRSI(IEnumerable<decimal> closingPrices, int period)
        {
            var enumerable = closingPrices as decimal[] ?? closingPrices.ToArray();
            var lowest = enumerable.Take(period).Min();
            var highest = enumerable.Take(period).Max();
            var current = enumerable.Last();

            var stochasticOscillator = (current - lowest) / (highest - lowest) * 100;

            return stochasticOscillator;
        }

    }
}
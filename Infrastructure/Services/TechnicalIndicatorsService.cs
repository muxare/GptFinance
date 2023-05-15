using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;

namespace GptFinance.Infrastructure.Services
{
    public class TechnicalIndicatorsService : ITechnicalIndicatorsService
    {
        private readonly AppDbContext _context;
        public TechnicalIndicatorsService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public decimal CalculateEMA(decimal previousEma, decimal currentPrice, int period)
        {
            decimal multiplier = 2.0M / (period + 1);
            return (currentPrice - previousEma) * multiplier + previousEma;
        }

        public (decimal macdLine, decimal signalLine) CalculateMACD(IEnumerable<decimal> closingPrices, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            var enumerable = closingPrices as decimal[] ?? closingPrices.ToArray();
            var shortEma = enumerable.Take(shortPeriod).Average();
            var longEma = enumerable.Take(longPeriod).Average();

            var macdLine = shortEma - longEma;
            var signalLine = enumerable.Skip(longPeriod).Take(signalPeriod).Average();

            return (macdLine, signalLine);
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


        public async Task CalculateAndStoreEma(int id, int period, Company company)
        {
            var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();
            var data = CalculateEma(id, period, closingPrices);

            _context.EmaData.AddRange(data);

            await _context.SaveChangesAsync();
        }

        // TODO: All series needs to be indexed by date
        private ICollection<EmaData> CalculateEma(int id, int period, List<decimal?> closingPrices)
        {
            var previousEma = (decimal)closingPrices.Take(period).Average();

            ICollection<EmaData> data = new List<EmaData>();
            for (int i = period; i < closingPrices.Count; i++)
            {
                var close = closingPrices[i];
                if (close == null)
                    continue;

                decimal ema = CalculateEMA(previousEma, close.Value, period);

                data.Add(new EmaData
                    {
                        CompanyId = id,
                        Value = ema,
                        Period = period
                    }
                );

                previousEma = ema;
            }

            return data;
        }

        public async Task CalculateAndStoreMacd(int id, int shortPeriod, int longPeriod, int signalPeriod, Company company)
        {
            var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();
            var emaLong = CalculateEma(id, longPeriod, closingPrices);
            var emaShort = CalculateEma(id, shortPeriod, closingPrices);



            int index = longPeriod - 1;
            foreach (var eodData in company.EodData.Skip(longPeriod - 1))
            {
                var (macdLine, signalLine) = CalculateMACD(
                    closingPrices.Take(index + 1).Where(x => x.HasValue).Select(x => x.Value), shortPeriod, longPeriod,
                    signalPeriod);

                _context.MacdData.Add(new MacdData
                {
                    CompanyId = id,
                    Date = eodData.Date,
                    Value = macdLine - signalLine,
                    ShortPeriod = shortPeriod,
                    LongPeriod = longPeriod,
                    SignalPeriod = signalPeriod
                });

                index++;
            }

            await _context.SaveChangesAsync();
        }
    }
}
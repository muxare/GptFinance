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

        public async Task CalculateAndStore(int id, int period, Company company)
        {
            var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();
            var previousEma = (decimal)closingPrices.Take(period).Average();
            var index = period;

            ICollection<EmaData> data = new List<EmaData>();
            foreach (var eodData in company.EodData.Skip(period))
            {
                if (eodData.Close == null)
                    continue;
                decimal ema = CalculateEMA(previousEma, eodData.Close.Value, period);

                data.Add(new EmaData
                    {
                        CompanyId = id,
                        Date = eodData.Date,
                        Value = ema,
                        Period = period
                    }
                );

                previousEma = ema;
                index++;
            }

            _context.EmaData.AddRange(data);

            await _context.SaveChangesAsync();
        }

        public async Task CalculateAndStoreMacd(int id, int shortPeriod, int longPeriod, int signalPeriod, Company company)
        {
            var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();

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
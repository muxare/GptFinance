namespace YahooFinanceAPI.Services
{
    // Services/YahooFinanceService.cs
    public class TechnicalIndicatorsService
    {
        public decimal CalculateEMA(decimal previousEMA, decimal currentPrice, int period)
        {
            decimal multiplier = 2.0M / (period + 1);
            return (currentPrice - previousEMA) * multiplier + previousEMA;
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
    }
}

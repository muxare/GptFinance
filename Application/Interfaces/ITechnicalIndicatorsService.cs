namespace YahooFinanceAPI.Services;

public interface ITechnicalIndicatorsService
{
    decimal CalculateEMA(decimal previousEMA, decimal currentPrice, int period);
    (decimal macdLine, decimal signalLine) CalculateMACD(IEnumerable<decimal> closingPrices, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9);
}
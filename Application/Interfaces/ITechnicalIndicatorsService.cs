using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface ITechnicalIndicatorsService
{
    //decimal CalculateEMA(decimal previousEMA, decimal currentPrice, int period);
    //(decimal macdLine, decimal signalLine) CalculateMACD(IEnumerable<decimal> closingPrices, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9);
    Task CalculateAndStoreEma(int id, int period, Company company);
    Task CalculateAndStoreMacd(int id, int shortPeriod, int longPeriod, int signalPeriod, Company company);
}
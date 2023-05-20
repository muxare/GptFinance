using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface ITechnicalIndicatorsService
{
    Task CalculateAndStoreEma(int period, Company company);
    Task CalculateAndStoreMacd(int shortPeriod, int longPeriod, int signalPeriod, Company company);
}
using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ITechnicalIndicatorsService
{
    Task CalculateAndStoreEma(int period, CompanyAggregate company);

    Task CalculateAndStoreEmaFan(int[] ints, ICollection<CompanyAggregate> companies);

    Task CalculateAndStoreMacd(int shortPeriod, int longPeriod, int signalPeriod, CompanyAggregate company);

    Task CalculateAndStoreMacdOnAllCompanies(int shortPeriod, int longPeriod, int signalPeriod, ICollection<CompanyAggregate> companies);
}

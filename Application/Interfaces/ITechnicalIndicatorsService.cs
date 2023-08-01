using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ITechnicalIndicatorsService
{
    Task CalculateAndStoreEma(int period, Company company);

    Task CalculateAndStoreEmaFan(int[] ints, ICollection<Company> companies);

    Task CalculateAndStoreMacd(int shortPeriod, int longPeriod, int signalPeriod, Company company);

    Task CalculateAndStoreMacdOnAllCompanies(int shortPeriod, int longPeriod, int signalPeriod, ICollection<Company> companies);
}

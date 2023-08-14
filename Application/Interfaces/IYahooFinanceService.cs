using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;

namespace GptFinance.Application.Interfaces;

public interface IYahooFinanceService<T>
{
    Task<CompanyAggregate> GetHistoricalDataAsync(CompanyAggregate company, DateTime startDate, DateTime endDate);

    List<EodDomainEntity> Convert(List<T> csvRecords, Guid companyId);

    Task<CompanyAggregate?> GetQuoteAsync(string? symbol);

    Task<ICollection<EodDomainEntity>> GetQuotesByCompanyId(Guid id);

    Task GetAllHistoricalDataAsync(ICollection<CompanyAggregate> companies, DateTime startDate, DateTime endDate);

    Task GetHistoricalDataAsync(ICollection<CompanyAggregate> companies);

    Task<IDictionary<Guid, EodDomainEntity>> GetLastEods();
}

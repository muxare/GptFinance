using GptFinance.Domain.Entity;
using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface IYahooFinanceService<T>
{
    Task<List<Eod>> GetHistoricalDataAsync(Company company, DateTime startDate, DateTime endDate);

    List<Eod> Convert(List<T> csvRecords, Guid companyId);

    Task<Company?> GetQuoteAsync(string? symbol);

    Task<ICollection<Eod>> GetQuotesByCompanyId(Guid id);

    Task GetAllHistoricalDataAsync(ICollection<Company> companies, DateTime startDate, DateTime endDate);

    Task<IDictionary<Guid, Eod>> GetLastEods();
}

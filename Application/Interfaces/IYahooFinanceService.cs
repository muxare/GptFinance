using GptFinance.Domain;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IYahooFinanceService<T>
{
    Task<List<EodData>> GetHistoricalDataAsync(Company company, DateTime startDate, DateTime endDate);
    List<EodData> Convert(List<T> csvRecords, Guid companyId);
    Task<Company?> GetQuoteAsync(string? symbol);
    Task<ICollection<EodData>> GetQuotesByCompanyId(Guid id);
    Task GetAllHistoricalDataAsync(ICollection<Company> companies, DateTime startDate, DateTime endDate);
    Task<IDictionary<Guid, EodData>> GetLastEods();
}
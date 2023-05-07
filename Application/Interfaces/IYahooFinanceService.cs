using GptFinance.Domain;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IYahooFinanceService<T>
{
    Task<List<EodData>> GetHistoricalDataAsync(Company company, DateTime startDate, DateTime endDate);
    List<EodData> Convert(List<T> csvRecords, int companyId);
    Task<Company?> GetQuoteAsync(string? symbol);
    Task<Maybe<ICollection<EodData>>> GetQuotesByCompanyId(int id);
}
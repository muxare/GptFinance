using GptFinance.Domain.Entities;


namespace YahooFinanceAPI.Services;

public interface IYahooFinanceService<T>
{
    Task<List<EodData>> GetHistoricalDataAsync(Company company, DateTime startDate, DateTime endDate);
    List<EodData> Convert(List<T> csvRecords, int companyId);
    Task<Company?> GetQuoteAsync(string? symbol);
}
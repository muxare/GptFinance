using GptFinance.Domain.Entities;


namespace YahooFinanceAPI.Services;

public interface IYahooFinanceService<T>
{
    Task<List<EodData>> GetHistoricalDataAsync(string? symbol, DateTime startDate, DateTime endDate);
    List<EodData> Convert(List<T> csvRecords);
    Task<Company?> GetQuoteAsync(string? symbol);
}
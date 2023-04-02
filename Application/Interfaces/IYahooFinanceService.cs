using YahooFinanceAPI.Models;


namespace YahooFinanceAPI.Services;

public interface IYahooFinanceService<T>
{
    Task<List<EODData>> GetHistoricalDataAsync(string? symbol, DateTime startDate, DateTime endDate);
    List<EODData> Convert(List<T> csvRecords);
    Task<Company?> GetQuoteAsync(string? symbol);
}
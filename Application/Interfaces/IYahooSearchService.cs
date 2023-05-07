using GptFinance.Application.Models;

namespace GptFinance.Application.Interfaces;

public interface IYahooSearchService<T>
{
    Task<List<YahooSearchResult>?> SearchCompaniesAsync(string query);
    Task<List<T>> SearchCompaniesAsync(IEnumerable<string> queries);
}
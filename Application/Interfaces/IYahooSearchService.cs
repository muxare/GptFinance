using GptFinance.Application.Models;

namespace GptFinance.Application.Interfaces;

public interface IYahooSearchService
{
    Task<SearchResult> SearchCompaniesAsync(string query);

    Task<List<SearchResult>> SearchCompaniesAsync(IEnumerable<string> queries);
}

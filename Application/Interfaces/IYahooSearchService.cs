using GptFinance.Application.Models.Yahoo;

namespace GptFinance.Application.Interfaces;

public interface IYahooSearchService
{
    Task<SearchResult> SearchCompaniesAsync(string query);

    Task<List<SearchResult>> SearchCompaniesAsync(IEnumerable<string> queries);
}

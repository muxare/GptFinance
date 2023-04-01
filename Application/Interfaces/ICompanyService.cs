using YahooFinanceAPI.Models;

namespace YahooFinanceAPI.Services;

public interface ICompanyService
{
    Task<Company> AddCompanyAsync(YahooSearchResult searchResult);
    Task AddMultipleCompaniesAsync(List<Company> companies);
}
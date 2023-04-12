using GptFinance.Domain.Entities;

namespace YahooFinanceAPI.Services;

public interface ICompanyService
{
    Task<Company> AddCompanyAsync(YahooSearchResult searchResult);
    Task AddMultipleCompaniesAsync(List<Company> companies);
    //Task UpsertEodDataAsync(List<EodData> eodDataList);
}
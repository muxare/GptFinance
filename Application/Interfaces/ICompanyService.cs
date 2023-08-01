using GptFinance.Application.Models;
using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ICompanyService
{
    Task<ICollection<Company>> GetAll();

    Task<Company> FindAsync(Guid id);

    Task<Company?> FindWithEodDataAsync(Guid id);

    Task<Company> AddCompanyAsync(YahooSearchResult searchResult);

    Task AddMultipleCompaniesAsync(List<Company> companies);

    Task UpdateCompany(Guid id, Company company);

    //Task UpdateCompanyData(Company company);
    Task DeleteCompany(Guid id);
}

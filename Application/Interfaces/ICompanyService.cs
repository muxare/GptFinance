using GptFinance.Application.Models;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface ICompanyService
{
    Task<ICollection<Company>> GetAll();
    Task<Company> FindAsync(int id);
    Task<Company?> FindWithEodDataAsync(int id);
    Task<Company> AddCompanyAsync(YahooSearchResult searchResult);
    Task AddMultipleCompaniesAsync(List<Company> companies);
    Task UpdateCompany(int id, Company company);
    //Task UpdateCompanyData(Company company);
    Task DeleteCompany(int id);
}
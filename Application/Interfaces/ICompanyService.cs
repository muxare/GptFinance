using GptFinance.Application.Models;
using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ICompanyService
{
    Task<ICollection<CompanyAggregate>> GetAll(int take = 1000);

    Task<CompanyAggregate> FindAsync(Guid id);

    Task<CompanyAggregate?> FindWithEodDataAsync(Guid id);

    Task<CompanyAggregate> AddCompanyAsync(YahooSearchResult searchResult);

    Task AddMultipleCompaniesAsync(List<CompanyAggregate> companies);

    Task UpdateCompany(Guid id, CompanyAggregate company);

    //Task UpdateCompanyData(Company company);
    Task DeleteCompany(Guid id);

    Task<ICollection<UpdateStatusAggregate>> GetCompanyUpdateStatus();
}

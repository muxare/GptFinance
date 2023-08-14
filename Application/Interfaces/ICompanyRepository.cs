using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ICompanyRepository //: IRepository<Company>
{
    bool Exists(Guid id);

    Task<int> DeleteByIdAsync(Guid id);

    Task<CompanyAggregate?> FindWithEodDataAsync(Guid id);

    Task<ICollection<CompanyAggregate>> GetAllAsync(int take = 1000);

    Task<CompanyAggregate> AddAsync(CompanyAggregate company);

    Task AddRange(ICollection<CompanyAggregate> companies);

    Task UpdateAsync(CompanyAggregate company);

    Task DeleteAsync(Guid id);

    Task<CompanyAggregate> GetByIdAsync(Guid id);
}

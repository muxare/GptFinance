using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ICompanyRepository //: IRepository<Company>
{
    bool Exists(Guid id);

    Task<int> DeleteByIdAsync(Guid id);

    Task<Company?> FindWithEodDataAsync(Guid id);

    Task<ICollection<Company>> GetAllAsync();

    Task<Company> AddAsync(Company company);

    Task AddRange(ICollection<Company> companies);

    Task UpdateAsync(Company company);

    Task DeleteAsync(Guid id);

    Task<Company> GetByIdAsync(Guid id);
}

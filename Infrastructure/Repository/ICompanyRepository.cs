using GptFinance.Domain.Entities;

namespace GptFinance.Infrastructure.Repository;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> FindWithEodDataAsync(int id);
}
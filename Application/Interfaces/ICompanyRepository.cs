using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> FindWithEodDataAsync(int id);
}
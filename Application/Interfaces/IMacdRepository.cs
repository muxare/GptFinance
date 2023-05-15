using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IMacdRepository
{
    Task<MacdData> GetByCompanyIdAndDateAsync(int companyId, DateTime date);
    Task<List<MacdData>> GetByCompanyIdAsync(int companyId);
    Task<MacdData> AddAsync(MacdData entity);
    Task AddRangeAsync(IEnumerable<MacdData> entities);
    Task<bool> ExistsAsync(int companyId, DateTime date);
}
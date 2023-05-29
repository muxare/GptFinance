using System.Linq.Expressions;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IMacdRepository
{
    Task<MacdData> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);
    Task<List<MacdData>> GetByCompanyIdAsync(Guid companyId);
    Task<MacdData> AddAsync(MacdData entity);
    Task AddRangeAsync(IEnumerable<MacdData> entities);
    Task<bool> ExistsAsync(Guid companyId, DateTime date);
    Task<List<MacdData>> GetAsync(Expression<Func<MacdData, bool>> filter);
}
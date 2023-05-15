using System.Linq.Expressions;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IEmaRepository
{
    Task<EmaData> GetByCompanyIdAndDateAsync(int companyId, DateTime date);
    Task<List<EmaData>> GetByCompanyIdAsync(int companyId);
    Task<EmaData> AddAsync(EmaData entity);
    Task AddRangeAsync(IEnumerable<EmaData> entities);
    Task<bool> ExistsAsync(int companyId, DateTime date);
    Task<List<EmaData>> GetAsync(Expression<Func<EmaData, bool>> filter);
}
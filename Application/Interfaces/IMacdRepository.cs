using GptFinance.Domain.Entity;
using System.Linq.Expressions;

namespace GptFinance.Application.Interfaces;

public interface IMacdRepository
{
    Task<Macd> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);

    Task<List<Macd>> GetByCompanyIdAsync(Guid companyId);

    Task<Macd> AddAsync(Macd entity);

    Task AddRangeAsync(IEnumerable<Macd> entities);

    Task<bool> ExistsAsync(Guid companyId, DateTime date);

    Task<List<Macd>> GetAsync(Expression<Func<Macd, bool>> filter);
}

using GptFinance.Domain.Entity;
using System.Linq.Expressions;

namespace GptFinance.Application.Interfaces;

public interface IEmaRepository
{
    Task<Ema> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);

    Task<List<Ema>> GetByCompanyIdAsync(Guid companyId);

    Task<Ema> AddAsync(Ema entity);

    Task AddRangeAsync(IEnumerable<Ema> entities);

    Task<bool> ExistsAsync(Guid companyId, DateTime date);

    Task<List<Ema>> GetAsync(Expression<Func<Ema, bool>> filter);

    Task<IDictionary<Guid, DateTime>> GetLastEodByCompany();
}

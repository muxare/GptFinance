using GptFinance.Domain.Entity;
using System.Linq.Expressions;

namespace GptFinance.Application.Interfaces;

public interface IMacdRepository
{
    Task<MacdDomainEntity> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);

    Task<List<MacdDomainEntity>> GetByCompanyIdAsync(Guid companyId);

    Task<MacdDomainEntity> AddAsync(MacdDomainEntity entity);

    Task AddRangeAsync(IEnumerable<MacdDomainEntity> entities);

    Task<bool> ExistsAsync(Guid companyId, DateTime date);

    Task<List<MacdDomainEntity>> GetAsync(Expression<Func<MacdDomainEntity, bool>> filter);
}

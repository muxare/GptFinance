using GptFinance.Domain.Entity;
using System.Linq.Expressions;

namespace GptFinance.Application.Interfaces;

public interface IEmaRepository
{
    Task<EmaDomainEntity> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);

    Task<List<EmaDomainEntity>> GetByCompanyIdAsync(Guid companyId);

    Task<EmaDomainEntity> AddAsync(EmaDomainEntity entity);

    Task AddRangeAsync(IEnumerable<EmaDomainEntity> entities);

    Task<bool> ExistsAsync(Guid companyId, DateTime date);

    Task<List<EmaDomainEntity>> GetAsync(Expression<Func<EmaDomainEntity, bool>> filter);

    Task<IDictionary<Guid, DateTime>> GetLastEodByCompany();
}

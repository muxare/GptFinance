using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;

namespace GptFinance.Application.Interfaces;

public interface IEodDataRepository // : IRepository<EodData>
{
    ValueTask<EodDomainEntity?> GetByIdAsync(Guid id);

    Task<List<EodDomainEntity>> GetAllAsync();

    Task<EodDomainEntity> AddAsync(EodDomainEntity entity);

    Task AddRange(ICollection<EodDomainEntity> entities);

    Task UpdateAsync(EodDomainEntity entity);

    Task UpdateRageAsync(CompanyAggregate company);

    Task DeleteAsync(Guid id);

    bool Exists(Guid id);

    Task<int> DeleteByIdAsync(Guid id);

    Task<List<EodDomainEntity>> GetQuotesByCompanyId(Guid id);

    Task<int> DeleteByCompanyId(Guid id);

    Task UpdateDataByCompanyId(int companyId, ICollection<EodDomainEntity> eodData);

    Task<IDictionary<Guid, EodDomainEntity>> GetLastEods();
}

using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;

namespace GptFinance.Application.Interfaces;

public interface IEodDataRepository // : IRepository<EodData>
{
    ValueTask<Eod?> GetByIdAsync(Guid id);

    Task<List<Eod>> GetAllAsync();

    Task<Eod> AddAsync(Eod entity);

    Task AddRange(ICollection<Eod> entities);

    Task UpdateAsync(Eod entity);

    Task UpdateRageAsync(ICollection<Eod> entities);

    Task DeleteAsync(Guid id);

    bool Exists(Guid id);

    Task<int> DeleteByIdAsync(Guid id);

    Task<List<Eod>> GetQuotesByCompanyId(Guid id);

    Task<int> DeleteByCompanyId(Guid id);

    Task UpdateDataByCompanyId(int companyId, ICollection<Eod> eodData);

    Task<IDictionary<Guid, Eod>> GetLastEods();
}

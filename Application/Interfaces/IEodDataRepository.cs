using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IEodDataRepository // : IRepository<EodData>
{
    ValueTask<EodData?> GetByIdAsync(Guid id);
    Task<List<EodData>> GetAllAsync();
    Task<EodData> AddAsync(EodData entity);
    Task AddRange(ICollection<EodData> entities);
    Task UpdateAsync(EodData entity);
    Task UpdateRageAsync(ICollection<EodData> entities);
    Task DeleteAsync(Guid id);
    bool Exists(Guid id);
    Task<int> DeleteByIdAsync(Guid id);
    Task<List<EodData>> GetQuotesByCompanyId(Guid id);
    Task<int> DeleteByCompanyId(Guid id);
    Task UpdateDataByCompanyId(int companyId, ICollection<EodData> eodData);
    Task<IDictionary<Guid, EodData>> GetLastEods();
}
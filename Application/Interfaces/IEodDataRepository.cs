using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IEodDataRepository // : IRepository<EodData>
{
    Task<EodData> GetByIdAsync(Guid id);
    Task<ICollection<EodData>> GetAllAsync();
    Task<EodData> AddAsync(EodData entity);
    Task AddRange(ICollection<EodData> entities);
    Task UpdateAsync(EodData entity);
    Task DeleteAsync(Guid id);
    bool Exists(Guid id);
    Task<int> DeleteByIdAsync(Guid id);
    Task<ICollection<EodData>> GetQuotesByCompanyId(Guid id);
    Task<int> DeleteByCompanyId(Guid id);
    Task UpdateDataByCompanyId(int companyId, ICollection<EodData> eodData);
}
using System.Linq.Expressions;
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IEmaRepository
{
    Task<EmaData> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date);
    Task<List<EmaData>> GetByCompanyIdAsync(Guid companyId);
    Task<EmaData> AddAsync(EmaData entity);
    Task AddRangeAsync(IEnumerable<EmaData> entities);
    Task<bool> ExistsAsync(Guid companyId, DateTime date);
    Task<List<EmaData>> GetAsync(Expression<Func<EmaData, bool>> filter);
    Task<IDictionary<Guid, DateTime>> GetLastEodByCompany();
}
using GptFinance.Domain.Entities;

namespace GptFinance.Application.Interfaces;

public interface IEodDataRepository : IRepository<EodData>
{
    Task<ICollection<EodData>> GetQuotesByCompanyId(int id);
    Task<int> DeleteByCompanyId(int id);
}
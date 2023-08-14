using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface ICompanyScreenerService
{
    Task<ICollection<CompanyAggregate>> ScreenAsync();
}

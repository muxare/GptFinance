using Flurl.Util;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Models.Entities;

namespace GptFinance.Infrastructure.Services;

public class CompanyScreenerService : ICompanyScreenerService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmaRepository _emaDataRepository;
    private readonly IMacdRepository _macdDataRepository;

    public CompanyScreenerService(IEmaRepository emaDataRepository, IMacdRepository macdDataRepository, ICompanyRepository companyRepository)
    {
        _emaDataRepository = emaDataRepository ?? throw new ArgumentNullException(nameof(emaDataRepository));
        _macdDataRepository = macdDataRepository ?? throw new ArgumentNullException(nameof(macdDataRepository));
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }

    //public async Task<ICollection<Company>> ScreenAsync(Expression<Func<EmaData, bool>> filter)
    public async Task<ICollection<CompanyAggregate>> ScreenAsync()
    {
        var companies = await _companyRepository.GetAllAsync();

        // Filter out companies that are in an up trend
        companies = companies.Where(c => c.InUptrend()).ToList();

        return companies;
    }
}

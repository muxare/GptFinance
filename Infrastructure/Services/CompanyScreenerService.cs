using System.Linq.Expressions;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;

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
    public async Task<ICollection<Company>> ScreenAsync()
    {
        var emas = await _emaDataRepository.GetAsync(e => e.Date == DateTime.Today);
        List<MacdData> macds = await _macdDataRepository.GetAsync(e => e.Date == DateTime.Today);

        var companies = await _companyRepository.GetAllAsync();
        return companies;
    }
}
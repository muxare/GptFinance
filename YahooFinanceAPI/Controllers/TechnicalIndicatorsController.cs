using GptFinance.Application.Interfaces;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace YahooFinanceAPI.Controllers;

[Route("api/indicators")]
[ApiController]
public class TechnicalIndicatorsController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ITechnicalIndicatorsService _technicalIndicatorsService;
    private readonly IEodDataRepository _eodDataRepository;

    public TechnicalIndicatorsController(ICompanyService companyService, ITechnicalIndicatorsService technicalIndicatorsService, IEodDataRepository eodDataRepository)
    {
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        _technicalIndicatorsService = technicalIndicatorsService ?? throw new ArgumentNullException(nameof(technicalIndicatorsService));
        _eodDataRepository = eodDataRepository ?? throw new ArgumentNullException(nameof(eodDataRepository));
    }

    [HttpPost("{id:Guid}/ema")]
    public async Task<IActionResult> CalculateEma(Guid id, int period)
    {
        // var company =
        var company = await _companyService.FindWithEodDataAsync(id);

        if (company == null)
        {
            return NotFound();
        }
        await _technicalIndicatorsService.CalculateAndStoreEma(period, company);

        return NoContent();
    }

    [HttpPost("{id:Guid}/emafan")]
    public async Task<IActionResult> CalculateEmaFan(Guid id)
    {
        // var company =
        var company = await _companyService.FindWithEodDataAsync(id);

        if (company == null)
        {
            return NotFound();
        }
        await _technicalIndicatorsService.CalculateAndStoreEmaFan(new int[] {18, 50, 100, 200}, new List<Company>{company});

        return NoContent();
    }

    [HttpPost("emafan")]
    public async Task<IActionResult> CalculateEmaFan()
    {
        var companies = await _companyService.GetAll();
        if (companies.Count == 0)
        {
            return NotFound();
        }
        await _technicalIndicatorsService.CalculateAndStoreEmaFan(new int[] {18, 50, 100, 200}, companies);

        return NoContent();
    }

    // POST: api/companies/5/macd
    [HttpPost("{id:Guid}/macd")]
    public async Task<IActionResult> CalculateMacd(Guid id, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
    {
        var company =  await _companyService.FindAsync(id);
        var eodData = await _eodDataRepository.GetQuotesByCompanyId(id);
        company.EodData = eodData;

        await _technicalIndicatorsService.CalculateAndStoreMacd(shortPeriod, longPeriod, signalPeriod, company);
        return NoContent();
    }

    // POST: api/companies/5/macd
    [HttpPost("macd")]
    public async Task<IActionResult> CalculateMacdOnAllCompanies(int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
    {
        var companies = await _companyService.GetAll();
        if (companies.Count == 0)
        {
            return NotFound();
        }

        foreach (var company in companies)
        {
            var eodData = await _eodDataRepository.GetQuotesByCompanyId(company.Id);
            company.EodData = eodData;
        }

        await _technicalIndicatorsService.CalculateAndStoreMacdOnAllCompanies(shortPeriod, longPeriod, signalPeriod, companies);
        return NoContent();
    }
}
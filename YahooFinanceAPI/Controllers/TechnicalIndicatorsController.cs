using GptFinance.Application.Interfaces;
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

    [HttpPost("{id}/ema")]
    public async Task<IActionResult> CalculateEma(int id, int period)
    {
        // var company =
        var company = await _companyService.FindWithEodDataAsync(id);

        if (company == null)
        {
            return NotFound();
        }
        await _technicalIndicatorsService.CalculateAndStoreEma(id, period, company);

        return NoContent();
    }


    // POST: api/companies/5/macd
    [HttpPost("{id}/macd")]
    public async Task<IActionResult> CalculateMacd(int id, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
    {
        var company =  await _companyService.FindAsync(id);
        var eodData = await _eodDataRepository.GetQuotesByCompanyId(id);
        company.EodData = eodData;

        await _technicalIndicatorsService.CalculateAndStoreMacd(id, shortPeriod, longPeriod, signalPeriod, company);
        return NoContent();
    }
}
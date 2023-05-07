using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YahooFinanceAPI.Controllers;

[Route("api/companies/{companyId}/indicators")]
[ApiController]
public class TechnicalIndicatorsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly ITechnicalIndicatorsService _technicalIndicatorsService;

    // ... Constructor, DI services, and private methods
    public TechnicalIndicatorsController(ICompanyService companyService, ITechnicalIndicatorsService technicalIndicatorsService, AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
        _technicalIndicatorsService = technicalIndicatorsService ?? throw new ArgumentNullException(nameof(technicalIndicatorsService));
    }

    // POST: api/companies/5/ema
    [HttpPost("{id}/ema")]
    public async Task<IActionResult> CalculateEma(int id, int period)
    {
        // var company =
        var company = await _companyService.FindWithEodDataAsync(id);

        if (company == null)
        {
            return NotFound();
        }

        await _technicalIndicatorsService.CalculateAndStore(id, period, company);
        return NoContent();
    }


    // POST: api/companies/5/macd
    [HttpPost("{id}/macd")]
    public async Task<IActionResult> CalculateMACD(int id, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
    {
        var company = await _context.Companies.Include(c => c.EodData).FirstOrDefaultAsync(c => c.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        await _technicalIndicatorsService.CalculateAndStoreMacd(id, shortPeriod, longPeriod, signalPeriod, company);
        return NoContent();
    }

}
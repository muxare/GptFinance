using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;
using YahooFinanceAPI.Services;

namespace YahooFinanceAPI.Controllers;

[Route("api/eoddata")]
[ApiController]
public class EodDataController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly YahooFinanceService _yahooFinanceService;
    private readonly CompanyService _companyService;

    public EodDataController(AppDbContext context, YahooFinanceService yahooFinanceService, CompanyService companyService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
    }

    // GET: api/companies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ICollection<EodData>>> GetEodData(int id)
    {
        var eodData = await _yahooFinanceService.GetQuotesByCompanyId(id).ConfigureAwait(false);
        if (eodData.HasValue )
        {

        }

        if (eodData == null || eodData.Count == 0)
        {
            return NotFound();
        }

        return Ok(eodData);
    }

    // POST: api/companies/5/historical
    [HttpPost("{id}/historical")]
    public async Task<ActionResult<IEnumerable<EodData>>> FetchHistoricalData(int id, DateTime? startDate, DateTime? endDate)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
        {
            return NotFound();
        }

        startDate ??= DateTime.MinValue;
        endDate ??= DateTime.UtcNow;

        var _ = await _yahooFinanceService.GetHistoricalDataAsync(company, startDate.Value, endDate.Value);

        return CreatedAtAction(nameof(GetEodData), new { id = company.Id }, null);
    }
}
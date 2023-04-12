using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;
using YahooFinanceAPI.Services;

namespace YahooFinanceAPI.Controllers;

[Route("api/companies/{companyId}/eoddata")]
[ApiController]
public class EodDataController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly YahooFinanceService _yahooFinanceService;

    // ... Constructor, DI services, and private methods
    public EodDataController(AppDbContext context, YahooFinanceService yahooFinanceService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
    }

    // GET: api/companies/5/eoddata
    [HttpGet("{id}/eoddata")]
    public async Task<ActionResult<IEnumerable<EodData>>> GetEODData(int id)
    {
        var eodData = await _context.EodData.Where(e => e.CompanyId == id).ToListAsync();

        if (eodData == null || eodData.Count == 0)
        {
            return NotFound();
        }

        return eodData;
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

        var eodDataList = await _yahooFinanceService.GetHistoricalDataAsync(company.Symbol, startDate.Value, endDate.Value);

        // Add company id to each EODData item
        foreach (var eodData in eodDataList)
        {
            eodData.CompanyId = id;
        }

        _context.EodData.AddRange(eodDataList);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEODData), new { id = company.Id }, eodDataList);
    }
}
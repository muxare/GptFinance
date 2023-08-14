using GptFinance.Application.Interfaces;
using GptFinance.Infrastructure.Models;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YahooFinanceAPI.Controllers;

[Route("api/eoddata")]
[ApiController]
public class EodDataController : ControllerBase
{
    private readonly IYahooFinanceService<CsvRecord> _yahooFinanceService;
    private readonly ICompanyService _companyService;

    public EodDataController(IYahooFinanceService<CsvRecord> yahooFinanceService, ICompanyService companyService)
    {
        _yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
        _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
    }

    // GET: api/eoddata/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ICollection<EodData>>> GetEodData(Guid id)
    {
        var eodData = await _yahooFinanceService.GetQuotesByCompanyId(id).ConfigureAwait(false);
        if (eodData.Count == 0)
        {
            return NotFound();
        }

        return Ok(eodData);
    }

    // POST: api/eoddata/update
    [HttpPost("update")]
    public async Task<ActionResult<IEnumerable<EodData>>> Update()
    {
        var companies = await _companyService.GetAll();

        if (!companies.Any())
        {
            return NotFound();
        }

        var latestEodByCompany = await _yahooFinanceService.GetLastEods();
        foreach (var company in companies)
        {
            DateTime startDate = latestEodByCompany.Keys.Contains(company.Id) ? latestEodByCompany[company.Id].Date.AddDays(-10) : DateTime.MinValue;
            DateTime endDate = DateTime.UtcNow;

            var _ = await _yahooFinanceService.GetHistoricalDataAsync(company, startDate, endDate);
        }

        return Ok();
    }

    // POST: api/eoddata/5/historical
    [HttpPost("{id:Guid}/historical")]
    public async Task<ActionResult<IEnumerable<EodData>>> FetchHistoricalData(Guid id, DateTime? startDate, DateTime? endDate)
    {
        var company = await _companyService.FindAsync(id);

        if (company == null)
        {
            return NotFound();
        }

        startDate ??= DateTime.MinValue;
        endDate ??= DateTime.UtcNow;

        var _ = await _yahooFinanceService.GetHistoricalDataAsync(company, startDate.Value, endDate.Value);

        return CreatedAtAction(nameof(GetEodData), new { id = company.Id }, null);
    }

    // POST: api/eoddata/historical
    [HttpPost("historical")]
    public async Task<ActionResult> FetchAllHistoricalData(DateTime? startDate, DateTime? endDate)
    {
        startDate ??= DateTime.MinValue;
        endDate ??= DateTime.UtcNow;

        var companies = await _companyService.GetAll();
        await _yahooFinanceService.GetAllHistoricalDataAsync(companies, startDate.Value, endDate.Value);

        return Ok();
    }

    // POST: api/eoddata/historical
    [HttpPost("updatehistory")]
    public async Task<ActionResult> FetchHistoricalData()
    {
        var companies = await _companyService.GetAll(1);
        await _yahooFinanceService.GetHistoricalDataAsync(companies.Where(o => o.MarketIsClosed()).ToList());

        return Ok();
    }
}

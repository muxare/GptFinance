using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;
using YahooFinanceAPI.Services;

namespace YahooFinanceAPI.Controllers;

[Route("api/companies/{companyId}/indicators")]
[ApiController]
public class TechnicalIndicatorsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TechnicalIndicatorsService _technicalIndicatorsService;

    // ... Constructor, DI services, and private methods
    public TechnicalIndicatorsController(AppDbContext context, TechnicalIndicatorsService technicalIndicatorsService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _technicalIndicatorsService = technicalIndicatorsService ?? throw new ArgumentNullException(nameof(technicalIndicatorsService));
    }

    // POST: api/companies/5/ema
    [HttpPost("{id}/ema")]
    public async Task<IActionResult> CalculateEma(int id, int period)
    {
        var company = await _context.Companies.Include(c => c.EodData).FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
        {
            return NotFound();
        }

        var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();
        var previousEma = (decimal)closingPrices.Take(period).Average();
        var index = period;

        foreach (var eodData in company.EodData.Skip(period))
        {
            if (eodData.Close == null)
                continue;
            decimal ema = _technicalIndicatorsService.CalculateEMA(previousEma, eodData.Close.Value, period);

            _context.EmaData.Add(new EmaData
            {
                CompanyId = id,
                Date = eodData.Date,
                Value = ema,
                Period = period
            });

            previousEma = ema;
            index++;
        }

        await _context.SaveChangesAsync();
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

        var closingPrices = company.EodData.OrderBy(e => e.Date).Select(e => e.Close).ToList();

        int index = longPeriod - 1;
        foreach (var eodData in company.EodData.Skip(longPeriod - 1))
        {
            var (macdLine, signalLine) = _technicalIndicatorsService.CalculateMACD(closingPrices.Take(index + 1).Where(x => x.HasValue).Select(x => x.Value), shortPeriod, longPeriod, signalPeriod);

            _context.MacdData.Add(new MacdData
            {
                CompanyId = id,
                Date = eodData.Date,
                Value = macdLine - signalLine,
                ShortPeriod = shortPeriod,
                LongPeriod = longPeriod,
                SignalPeriod = signalPeriod
            });

            index++;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
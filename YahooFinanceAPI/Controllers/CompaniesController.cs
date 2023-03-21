using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;
using YahooFinanceAPI.Models;
using YahooFinanceAPI.Services;

namespace YahooFinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly YahooFinanceService _yahooFinanceService;
        private readonly TechnicalIndicatorsService _technicalIndicatorsService;
        private readonly YahooSearchService _yahooSearchService;
        private readonly CompanyService _companyService;

        public CompaniesController(AppDbContext context,
            YahooFinanceService yahooFinanceService,
            TechnicalIndicatorsService technicalIndicatorsService,
            YahooSearchService yahooSearchService,
            CompanyService companyService)
        {
            _context = context;
            _yahooFinanceService = yahooFinanceService;
            _technicalIndicatorsService = technicalIndicatorsService;
            _yahooSearchService = yahooSearchService;
            _companyService = companyService;
        }

        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // GET: api/companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/companies
        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(YahooSearchResult searchResult)
        {
            var company = await _companyService.AddCompanyAsync(searchResult);

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        [HttpPost("add-multiple")]
        public async Task<IActionResult> AddMultipleCompaniesAsync([FromBody] List<Company> companies)
        {
            if (companies == null || companies.Count == 0)
            {
                return BadRequest("No companies provided");
            }

            await _companyService.AddMultipleCompaniesAsync(companies);

            return Ok("Companies added successfully");
        }

        // DELETE: api/companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/companies/5/fetch
        [HttpPost("{id}/fetch")]
        public async Task<ActionResult<Company>> FetchCompanyData(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            var quote = await _yahooFinanceService.GetQuoteAsync(company.Symbol);

            if (quote == null)
            {
                return BadRequest("Failed to fetch data from Yahoo Finance");
            }

            company.LastUpdated = DateTime.Now;

            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return company;
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        // GET: api/companies/5/eoddata
        [HttpGet("{id}/eoddata")]
        public async Task<ActionResult<IEnumerable<EODData>>> GetEODData(int id)
        {
            var eodData = await _context.EODData.Where(e => e.CompanyId == id).ToListAsync();

            if (eodData == null || eodData.Count == 0)
            {
                return NotFound();
            }

            return eodData;
        }

        // POST: api/companies/5/historical
        [HttpPost("{id}/historical")]
        public async Task<ActionResult<IEnumerable<EODData>>> FetchHistoricalData(int id, DateTime? startDate, DateTime? endDate)
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

            _context.EODData.AddRange(eodDataList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEODData), new { id = company.Id }, eodDataList);
        }

        // POST: api/companies/5/ema
        [HttpPost("{id}/ema")]
        public async Task<IActionResult> CalculateEMA(int id, int period)
        {
            var company = await _context.Companies.Include(c => c.EODData).FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var closingPrices = company.EODData.OrderBy(e => e.Date).Select(e => e.Close).ToList();
            decimal previousEMA = (decimal)closingPrices.Take(period).Average();
            int index = period;

            foreach (var eodData in company.EODData.Skip(period))
            {
                if (eodData.Close == null)
                    continue;
                decimal ema = _technicalIndicatorsService.CalculateEMA(previousEMA, eodData.Close.Value, period);

                _context.EMAData.Add(new EMAData
                {
                    CompanyId = id,
                    Date = eodData.Date,
                    Value = ema,
                    Period = period
                });

                previousEMA = ema;
                index++;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/companies/5/macd
        [HttpPost("{id}/macd")]
        public async Task<IActionResult> CalculateMACD(int id, int shortPeriod = 12, int longPeriod = 26, int signalPeriod = 9)
        {
            var company = await _context.Companies.Include(c => c.EODData).FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            var closingPrices = company.EODData.OrderBy(e => e.Date).Select(e => e.Close).ToList();

            int index = longPeriod - 1;
            foreach (var eodData in company.EODData.Skip(longPeriod - 1))
            {
                var (macdLine, signalLine) = _technicalIndicatorsService.CalculateMACD(closingPrices.Take(index + 1).Where(x => x.HasValue).Select(x => x.Value), shortPeriod, longPeriod, signalPeriod);

                _context.MACDData.Add(new MACDData
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchCompanies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("The query parameter is required.");
            }

            var results = await _yahooSearchService.SearchCompaniesAsync(query);
            return Ok(results);
        }

        [HttpGet("search-multiple")]
        public async Task<IActionResult> SearchCompaniesMultiple(string queries)
        {
            if (string.IsNullOrWhiteSpace(queries))
            {
                return BadRequest("The 'queries' parameter is required.");
            }

            var searchQueries = queries.Split(',').Select(q => q.Trim());
            var results = await _yahooSearchService.SearchCompaniesAsync(searchQueries);
            return Ok(results);
        }
    }
}

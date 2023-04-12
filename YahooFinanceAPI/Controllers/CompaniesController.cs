using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;
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

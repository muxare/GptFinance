using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YahooFinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IYahooSearchService<Company> _yahooSearchService;
        private readonly ICompanyService _companyService;

        public CompaniesController(
            IYahooSearchService<Company> yahooSearchService,
            ICompanyService companyService)
        {
            _yahooSearchService = yahooSearchService;
            _companyService = companyService;
        }

        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult<ICollection<Company>>> GetCompanies()
        {
            return Ok(await _companyService.GetAll());
        }

        // GET: api/companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _companyService.FindAsync(id);

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

            await _companyService.UpdateCompany(id, company);
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
            await _companyService.DeleteCompany(id);

            return NoContent();
        }

        // POST: api/companies/5/fetch
        /*
        [HttpPost("{id}/fetch")]
        public async Task<ActionResult<Company>> FetchCompanyData(int id)
        {
            var company = await _companyService.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            await _companyService.UpdateCompanyData(company);

            return company;
        }
        */


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

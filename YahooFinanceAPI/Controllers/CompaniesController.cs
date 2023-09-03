using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Application.Models.Dto;
using GptFinance.Domain.Aggregate;
using GptFinance.Infrastructure.Mappings;
using Microsoft.AspNetCore.Mvc;
using YahooFinanceAPI.Dto;

namespace YahooFinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IYahooSearchService _yahooSearchService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            IYahooSearchService yahooSearchService,
            ICompanyService companyService,
            ILogger<CompaniesController> logger)
        {
            _yahooSearchService = yahooSearchService;
            _companyService = companyService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/companies
        [HttpGet]
        public async Task<ActionResult> GetCompanies()
        {
            return Ok(await _companyService.GetAll());
        }

        // GET: api/companies/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult> GetCompany(Guid id)
        {
            CompanyAggregate company = await _companyService.FindAsync(id);

            return Ok(company);
        }

        // PUT: api/companies/5
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateCompany(Guid id, CompanyDto company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            var companyAggregate = new CompanyAggregate
            {
                Id = id,
                Symbol = company.Symbol,
                Name = company.Name
            };

            await _companyService.UpdateCompany(id, companyAggregate);
            return NoContent();
        }

        // POST: api/companies
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(YahooSearchResult searchResult)
        {
            _logger.LogWarning("Agruments: {searchResult}", searchResult);
            var company = await _companyService.AddCompanyAsync(searchResult);

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        [HttpPost("add-multiple")]
        public async Task<ActionResult> AddMultipleCompaniesAsync([FromBody] List<CompanySearchDto> companies)
        {
            if (companies == null || companies.Count == 0)
            {
                return BadRequest("No companies provided");
            }
            var companyAggregates = companies.Select(c => c.Map()).ToList();
            await _companyService.AddMultipleCompaniesAsync(companyAggregates);

            return Ok("Companies added successfully");
        }

        // DELETE: api/companies/5
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteCompany(Guid id)
        {
            await _companyService.DeleteCompany(id);

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchCompanies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("The query parameter is required.");
            }

            var results = await _yahooSearchService.SearchCompaniesAsync(query);
            return Ok(results.ToYahooCompanySearchResult());
        }

        /// <summary>
        /// Search for multiple companies by comma separated tickers
        /// ^OMXS30: SHB-A.ST, SWED-A.ST, SEB-A.ST, SCA-B.ST, ATCO-A.ST, GETI-B.ST, HEXA-B.ST, NDA-SE.ST, ALIV-SDB.ST, AZN.ST, SINCH.ST, HM-B.ST, TEL2-B.ST, ABB.ST, SAND.ST, SKF-B.ST, ASSA-B.ST, VOLV-B.ST, INVE-B.ST, ALFA.ST, ERIC-B.ST, EVO.ST, ESSITY-B.ST, ATCO-B.ST, BOL.ST, TELIA.ST, ELUX-B.ST, SBB-B.ST, NIBE-B.ST, KINV-B.ST
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        [HttpGet("search-multiple")]
        public async Task<ActionResult> SearchCompaniesMultiple(string queries)
        {
            if (string.IsNullOrWhiteSpace(queries))
            {
                return BadRequest("The 'queries' parameter is required.");
            }

            var searchQueries = queries.Split(',').Select(q => q.Trim());
            var results = await _yahooSearchService.SearchCompaniesAsync(searchQueries);
            return Ok(results.ToYahooCompanySearchResult());
        }

        [HttpGet("stats")]
        public async Task<ActionResult> CompaniesStats()
        {
            var results = await _companyService.GetCompanyUpdateStatus();
            return Ok(results);
        }
    }
}

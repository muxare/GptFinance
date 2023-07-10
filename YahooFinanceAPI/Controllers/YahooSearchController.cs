using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace YahooFinanceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class YahooSearchController : ControllerBase
{
    private readonly IYahooSearchService<Company> _yahooSearchService;

    public YahooSearchController(IYahooSearchService<Company> yahooSearchService)
    {
        _yahooSearchService = yahooSearchService;
    }

    [HttpGet("search/{query}")]
    public async Task<IActionResult> SearchCompanies(string query)
    {
        var results = await _yahooSearchService.SearchCompaniesAsync(query);
        return Ok(results);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchCompanies([FromBody] List<string> queries)
    {
        var results = await _yahooSearchService.SearchCompaniesAsync(queries);
        return Ok(results);
    }
}
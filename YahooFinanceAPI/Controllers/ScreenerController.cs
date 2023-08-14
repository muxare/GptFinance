using GptFinance.Application.Interfaces;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using YahooFinanceAPI.Dto;

namespace YahooFinanceAPI.Controllers;

[Route("api/screener")]
[ApiController]
public class ScreenerController : ControllerBase
{
    private readonly ICompanyScreenerService _companyScreenerService;

    public ScreenerController(ICompanyScreenerService companyScreenerService)
    {
        _companyScreenerService = companyScreenerService ?? throw new ArgumentNullException(nameof(companyScreenerService));
    }

    /// <summary>
    /// List all companies and order by the most interesting to lest interesting according to techical indicators and other trends
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ICollection<Company>>> List(FilterDto filter)
    {
        var res = await _companyScreenerService.ScreenAsync();
        return Ok(res);
    }
}

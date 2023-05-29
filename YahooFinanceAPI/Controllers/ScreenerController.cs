using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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


    // POST: api/screener/uptrend
    [HttpPost("uptrend")]
    public async Task<ActionResult<ICollection<Company>>> Uptrend()
    {
        var res= await _companyScreenerService.ScreenAsync();
        return Ok(res);
    }

}
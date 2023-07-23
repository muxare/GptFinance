namespace GptFinance.Application.Models;

public class CompanySearchDto
{
    public string ExchangeName { get; set; }
    public string ShortName { get; set; }
    public string QuoteType { get; set; }
    public string Symbol { get; set; }
    public string Index { get; set; }
    public string TypeDisp { get; set; }
    public string LongName { get; set; }
    public string ExchDisp { get; set; }
    public string Sector { get; set; }
    public string Industry { get; set; }
    public bool DispSecIndFlag { get; set; }
    public bool IsYahooFinance { get; set; }
}

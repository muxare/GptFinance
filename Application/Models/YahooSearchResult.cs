using Newtonsoft.Json;

namespace GptFinance.Application.Models;

public class YahooSearchResult
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("shortname")]
    public string? CompanyName { get; set; }
}
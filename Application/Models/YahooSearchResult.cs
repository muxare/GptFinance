using Newtonsoft.Json;

namespace YahooFinanceAPI.Services;

public class YahooSearchResult
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("shortname")]
    public string? CompanyName { get; set; }
}
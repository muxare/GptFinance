using Newtonsoft.Json;

namespace YahooFinanceAPI.Services;

public class YahooSearchResults
{
    [JsonProperty("quotes")]
    public List<YahooSearchResult> Quotes { get; set; }
}
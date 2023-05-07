using Newtonsoft.Json;

namespace GptFinance.Application.Models;

public class YahooSearchResults
{
    [JsonProperty("quotes")]
    public List<YahooSearchResult>? Quotes { get; set; }
}
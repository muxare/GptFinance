using Newtonsoft.Json;

namespace GptFinance.Application.Models;

public record YahooSearchResult
{
    [JsonProperty("symbol")]
    public string? Symbol { get; set; }

    [JsonProperty("shortname")]
    public string? CompanyName { get; set; }
}

public class SearchResult
{
    public List<object> explains { get; set; }
    public int count { get; set; }
    public List<Quote> quotes { get; set; }
    public List<News> news { get; set; }
}

public class Quote
{
    public string exchange { get; set; }
    public string shortname { get; set; }
    public string quoteType { get; set; }
    public string symbol { get; set; }
    public string index { get; set; }
    public double score { get; set; }
    public string typeDisp { get; set; }
    public string longname { get; set; }
    public string exchDisp { get; set; }
    public string sector { get; set; }
    public string industry { get; set; }
    public bool dispSecIndFlag { get; set; }
    public bool isYahooFinance { get; set; }
}

public class Resolution
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string tag { get; set; }
}

public class Thumbnail
{
    public List<Resolution> resolutions { get; set; }
}

public class News
{
    public string uuid { get; set; }
    public string title { get; set; }
    public string publisher { get; set; }
    public string link { get; set; }
    public int providerPublishTime { get; set; }
    public string type { get; set; }
    public Thumbnail thumbnail { get; set; }
    public List<string> relatedTickers { get; set; }
}

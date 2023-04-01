namespace YahooFinanceAPI.Services
{
    using Flurl;
    using Flurl.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using YahooFinanceAPI.Models;

    public class YahooSearchService : IYahooSearchService<Company>
    {
        private const string YahooFinanceApiBaseUrl = "https://query1.finance.yahoo.com";

        private readonly HttpClient _httpClient;

        public YahooSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<YahooSearchResult>> SearchCompaniesAsync(string query)
        {
            var response = await $"{YahooFinanceApiBaseUrl}/v1/finance/search"
                .SetQueryParams(new { q = query, quotesCount = 10, newsCount = 0 })
                .GetStringAsync();

            var searchResults = JsonConvert.DeserializeObject<YahooSearchResults>(response);
            return searchResults.Quotes;
        }

        public async Task<List<Company>> SearchCompaniesAsync(IEnumerable<string> queries)
        {
            var companies = new List<Company>();

            foreach (var query in queries)
            {
                var response = await _httpClient.GetAsync($"https://query1.finance.yahoo.com/v1/finance/search?q={Uri.EscapeDataString(query)}&quotesCount=10&newsCount=0");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonResponse);
                    var quoteResults = jsonObject["quotes"] as JArray;

                    if (quoteResults != null)
                    {
                        foreach (var quote in quoteResults)
                        {
                            var symbol = quote["symbol"]?.ToString();
                            var name = quote["shortname"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(symbol) && !string.IsNullOrWhiteSpace(name))
                            {
                                companies.Add(new Company
                                {
                                    Symbol = symbol,
                                    Name = name
                                });
                            }
                        }
                    }
                }
            }

            // Removing duplicates
            return companies.GroupBy(c => c.Symbol).Select(g => g.First()).ToList();
        }
    }
}

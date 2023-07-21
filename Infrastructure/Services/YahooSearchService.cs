using Flurl;
using Flurl.Http;
using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GptFinance.Infrastructure.Services
{
    public class YahooSearchService : IYahooSearchService
    {
        private const string YahooFinanceApiBaseUrl = "https://query1.finance.yahoo.com";

        private readonly HttpClient _httpClient;

        public YahooSearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResult> SearchCompaniesAsync(string query)
        {
            var response = await $"{YahooFinanceApiBaseUrl}/v1/finance/search"
                .SetQueryParams(new { q = query, quotesCount = 10, newsCount = 0 })
                .GetStringAsync();

            var searchResults = JsonConvert.DeserializeObject<SearchResult>(response);

            return searchResults ?? new SearchResult();
        }

        public async Task<List<SearchResult>> SearchCompaniesAsync(IEnumerable<string> queries)
        {
            var companies = new List<SearchResult>();

            foreach (var query in queries)
            {
                var response = await _httpClient.GetAsync($"https://query1.finance.yahoo.com/v1/finance/search?q={Uri.EscapeDataString(query)}&quotesCount=10&newsCount=0");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonResponse);
                    var quoteResults = jsonObject["quotes"] as JArray;

                    SearchResult myDeserializedClass = JsonConvert.DeserializeObject<SearchResult>(jsonResponse);
                    companies.Add(myDeserializedClass);
                }
            }

            return companies;
        }
    }
}

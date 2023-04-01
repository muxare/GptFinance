namespace YahooFinanceAPI.Services
{
    // Services/YahooFinanceService.cs

    using CsvHelper;
    using CsvHelper.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;

    public class YahooFinanceService : IYahooFinanceService<CsvRecord>
    {
        private const string BaseUrl = "https://query1.finance.yahoo.com/v7/finance/download/";
        private readonly HttpClient _httpClient;

        public YahooFinanceService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<EODData>> GetHistoricalDataAsync(string symbol, DateTime startDate, DateTime endDate)
        {
            string url = $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1={ToUnixTimestamp(startDate)}&period2={ToUnixTimestamp(endDate)}&interval=1d&events=history&includeAdjustedClose=true";

            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null
                    };
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    using (var csvReader = new CsvReader(reader, config))
                    {
                        csvReader.Context.RegisterClassMap<CsvRecordMap>();
                        var r = csvReader.GetRecords<CsvRecord>();
                        var records = new List<EODData>(Convert(csvReader.GetRecords<CsvRecord>().ToList()));
                        return records;
                    }
                }
                else
                {
                    throw new Exception($"Failed to fetch historical data for {symbol}: {response.ReasonPhrase}");
                }
            }
        }

        public List<EODData> Convert(List<CsvRecord> csvRecords)
        {
            return csvRecords.Select(r =>
            {
                return new EODData
                {
                    Id = 0,
                    Date = r.Date,
                    Open = r.Open.HasValue ? r.Open.Value : (decimal?)null,
                    High = r.High.HasValue ? r.High.Value : (decimal?)null,
                    Low = r.Low.HasValue ? r.Low.Value : (decimal?)null,
                    Close = r.Close.HasValue ? r.Close.Value : (decimal?)null,
                    Volume = r.Volume.HasValue ? r.Volume.Value : (long?)null
                };
            }).ToList();
        }

        /*
        private static Configuration GetCsvConfiguration()
        {
            var config = new Configuration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
            return config;
        }
        */

        private static long ToUnixTimestamp(DateTime dateTime)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - unixEpoch).TotalSeconds;
        }

        public async Task<Company> GetQuoteAsync(string symbol)
        {
            var httpClient = new HttpClient();
            var url = $"{BaseUrl}{symbol}?interval=1d&events=history&includeAdjustedClose=true";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            //csvReader.Configuration.HeaderValidated = null;
            //csvReader.Configuration.MissingFieldFound = null;

            var quote = csvReader.GetRecords<Company>().FirstOrDefault();

            return quote;
        }
    }

    public class CsvRecord
    {
        public DateTime Date { get; set; }

        public decimal? Open { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
        public decimal? Close { get; set; }
        public decimal? AdjClose { get; set; }
        public long? Volume { get; set; }
    }
}

using GptFinance.Domain;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using YahooFinanceAPI.Data;

namespace YahooFinanceAPI.Services
{
    // Services/YahooFinanceService.cs

    using CsvHelper;
    using CsvHelper.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class YahooFinanceService : IYahooFinanceService<CsvRecord>
    {
        private readonly AppDbContext _context;
        private const string BaseUrl = "https://query1.finance.yahoo.com/v7/finance/download/";
        private readonly HttpClient _httpClient;

        public YahooFinanceService(AppDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // TODO: The argument should be Company entity instead of ust the symbol.
        public async Task<List<EodData>> GetHistoricalDataAsync(Company company, DateTime startDate, DateTime endDate)
        {
            string url = $"https://query1.finance.yahoo.com/v7/finance/download/{company.Symbol}?period1={ToUnixTimestamp(startDate)}&period2={ToUnixTimestamp(endDate)}&interval=1d&events=history&includeAdjustedClose=true";

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
                        var records = new List<EodData>(Convert(csvReader.GetRecords<CsvRecord>().ToList(), company.Id));

                        _context.EodData.AddRange(records);
                        await _context.SaveChangesAsync();

                        return records;
                    }
                }
                else
                {
                    throw new Exception($"Failed to fetch historical data for {company.Symbol}: {response.ReasonPhrase}");
                }
            }
        }



        public List<EodData> Convert(List<CsvRecord> csvRecords, int companyId)
        {
            return csvRecords.Select(r =>
            {
                return new EodData
                {
                    Id = 0,
                    Date = r.Date,
                    Open = r.Open.HasValue ? r.Open.Value : (decimal?)null,
                    High = r.High.HasValue ? r.High.Value : (decimal?)null,
                    Low = r.Low.HasValue ? r.Low.Value : (decimal?)null,
                    Close = r.Close.HasValue ? r.Close.Value : (decimal?)null,
                    Volume = r.Volume.HasValue ? r.Volume.Value : (long?)null,
                    CompanyId = companyId
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

        public async Task<Company?> GetQuoteAsync(string? symbol)
        {
            var httpClient = new HttpClient();
            var url = $"{BaseUrl}{symbol}?interval=1d&events=history&includeAdjustedClose=true";

            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);

            //csvReader.Configuration.HeaderValidated = null;
            //csvReader.Configuration.MissingFieldFound = null;

            var quote = csvReader.GetRecords<Company>().FirstOrDefault();

            return quote;
        }

        public async Task<Maybe<ICollection<EodData>>> GetQuotesByCompanyId(int id)
        {
            var eodData = await _context.EodData.Where(e => e.CompanyId == id).OrderByDescending(o => o.Date).Take(100).ToListAsync();
            return eodData.ToMaybe<ICollection<EodData>>();
        }
    }

    // public class CsvRecord
    // {
    //     public DateTime Date { get; set; }
    //
    //     public decimal? Open { get; set; }
    //     public decimal? High { get; set; }
    //     public decimal? Low { get; set; }
    //     public decimal? Close { get; set; }
    //     public decimal? AdjClose { get; set; }
    //     public long? Volume { get; set; }
    // }
}

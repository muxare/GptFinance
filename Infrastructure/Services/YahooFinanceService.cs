using CsvHelper;
using CsvHelper.Configuration;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models;
using GptFinance.Infrastructure.Models.Entities;

namespace GptFinance.Infrastructure.Services
{
    public class YahooFinanceService : IYahooFinanceService<CsvRecord>
    {
        private readonly IEodDataRepository _eodRepository;

        //private readonly ICompanyService _companyService;
        private const string BaseUrl = "https://query1.finance.yahoo.com/v7/finance/download/";

        private readonly HttpClient _httpClient;

        public YahooFinanceService(IEodDataRepository eodRepository)
        {
            _eodRepository = eodRepository ?? throw new ArgumentNullException(nameof(eodRepository));
            //_companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _httpClient = new HttpClient();
        }

        public async Task<CompanyAggregate> GetHistoricalDataAsync(CompanyAggregate company, DateTime startDate, DateTime endDate)
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
                        //var r = csvReader.GetRecords<CsvRecord>();
                        var records = new List<EodDomainEntity>(Convert(csvReader.GetRecords<CsvRecord>().ToList(), company.Id));
                        company.FinancialData.EodData = records;
                        await _eodRepository.UpdateRageAsync(company);
                        //await _eodRepository.AddRange(records);

                        company.FinancialData.EodData = records;

                        return company;
                    }
                }
                else
                {
                    throw new Exception($"Failed to fetch historical data for {company.Symbol}: {response.ReasonPhrase}");
                }
            }
        }

        public List<EodDomainEntity> Convert(List<CsvRecord> csvRecords, Guid companyId)
        {
            return csvRecords.Select(r =>
            {
                return new EodDomainEntity
                (
                    Date: r.Date,
                    Open: r.Open.HasValue ? r.Open.Value : (decimal?)null,
                    High: r.High.HasValue ? r.High.Value : (decimal?)null,
                    Low: r.Low.HasValue ? r.Low.Value : (decimal?)null,
                    Close: r.Close.HasValue ? r.Close.Value : (decimal?)null,
                    AdjClose: r.Close.HasValue ? r.Close.Value : (decimal?)null,
                    Volume: r.Volume.HasValue ? (int?)r.Volume.Value : (int?)null
                );
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

        public async Task<CompanyAggregate?> GetQuoteAsync(string? symbol)
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

            var quote = csvReader.GetRecords<CompanyAggregate>().FirstOrDefault();

            return quote;
        }

        public async Task<ICollection<EodDomainEntity>> GetQuotesByCompanyId(Guid id)
        {
            ICollection<EodDomainEntity> eodData = await _eodRepository.GetQuotesByCompanyId(id);
            return eodData;
        }

        /// <summary>
        /// Deletes all historical data and fetches new data for all companies
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task GetAllHistoricalDataAsync(ICollection<CompanyAggregate> companies, DateTime startDate, DateTime endDate)
        {
            foreach (var company in companies)
            {
                await _eodRepository.DeleteByCompanyId(company.Id);
                var data = await GetHistoricalDataAsync(company, startDate, endDate);
                //await _eodRepository.AddRange(data);
            }
        }

        /// <summary>
        /// Deletes all historical data and fetches new data for all companies
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task GetHistoricalDataAsync(ICollection<CompanyAggregate> companies)
        {
            var now = DateTime.UtcNow;
            foreach (var company in companies)
            {
                var startDate = company.FinancialData.EodData.Any() ? company.FinancialData.EodData.Max(e => e.Date).AddDays(1) : DateTime.MinValue;
                // Also check if the stock market is open or closed for trade of the company, only get eod data if it is losed
                if (startDate <= now)
                    await GetHistoricalDataAsync(company, startDate, now);
            }
        }

        public async Task<IDictionary<Guid, EodDomainEntity>> GetLastEods()
        {
            IDictionary<Guid, EodDomainEntity> res = await _eodRepository.GetLastEods();

            return res;
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

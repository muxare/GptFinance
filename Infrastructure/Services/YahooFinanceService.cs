﻿using CsvHelper;
using CsvHelper.Configuration;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models;

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

                        await _eodRepository.UpdateRageAsync(records);
                        //await _eodRepository.AddRange(records);

                        return records;
                    }
                }
                else
                {
                    throw new Exception($"Failed to fetch historical data for {company.Symbol}: {response.ReasonPhrase}");
                }
            }
        }

        public List<EodData> Convert(List<CsvRecord> csvRecords, Guid companyId)
        {
            return csvRecords.Select(r =>
            {
                return new EodData
                {
                    Id = Guid.NewGuid(),
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

        public async Task<ICollection<EodData>> GetQuotesByCompanyId(Guid id)
        {
            ICollection<EodData> eodData = await _eodRepository.GetQuotesByCompanyId(id);
            return eodData;
        }

        /// <summary>
        /// Deletes all historical data and fetches new data for all companies
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task GetAllHistoricalDataAsync(ICollection<Company> companies, DateTime startDate, DateTime endDate)
        {
            foreach (var company in companies)
            {
                await _eodRepository.DeleteByCompanyId(company.Id);
                var data = await GetHistoricalDataAsync(company, startDate, endDate);
                //await _eodRepository.AddRange(data);
            }
        }

        public async Task<IDictionary<Guid, EodData>> GetLastEods()
        {
            IDictionary<Guid, EodData> res = await _eodRepository.GetLastEods();

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

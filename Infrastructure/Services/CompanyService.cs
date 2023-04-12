using GptFinance.Domain.Entities;

namespace YahooFinanceAPI.Services
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using YahooFinanceAPI.Data;

    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _dbContext;

        public CompanyService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Company> AddCompanyAsync(YahooSearchResult searchResult)
        {
            var company = new Company
            {
                Symbol = searchResult.Symbol,
                Name = searchResult.CompanyName,
                LastUpdated = DateTime.UtcNow
            };

            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            return company;
        }

        public async Task AddMultipleCompaniesAsync(List<Company> companies)
        {
            await _dbContext.Companies.AddRangeAsync(companies);
            await _dbContext.SaveChangesAsync();
        }

        /*
        public async Task UpsertEodDataAsync(List<EodData> eodDataList)
        {
            foreach (var eodData in eodDataList)
            {
                var existingEodData = _dbContext.EodData
                    .FirstOrDefault(d => d.CompanyId == eodData.CompanyId && d.Date == eodData.Date);

                if (existingEodData != null)
                {
                    // Update the existing record
                    existingEodData.Open = eodData.Open;
                    existingEodData.High = eodData.High;
                    existingEodData.Low = eodData.Low;
                    existingEodData.Close = eodData.Close;
                    existingEodData.Volume = eodData.Volume;
                }
                else
                {
                    // Insert new record
                    _dbContext.EodData.Add(eodData);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        */
    }
}

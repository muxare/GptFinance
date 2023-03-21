namespace YahooFinanceAPI.Services
{
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using YahooFinanceAPI.Data;
    using YahooFinanceAPI.Models;

    public class CompanyService
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
    }
}

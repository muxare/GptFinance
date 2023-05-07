using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IYahooFinanceService<CsvRecord> _yahooFinanceService;

        public CompanyService(AppDbContext context, IYahooFinanceService<CsvRecord> yahooFinanceService)
        {
            _context = context;
            _yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
        }

        public async Task<ICollection<Company>> GetAll()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company> AddCompanyAsync(YahooSearchResult searchResult)
        {
            var company = new Company
            {
                Symbol = searchResult.Symbol,
                Name = searchResult.CompanyName,
                LastUpdated = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task AddMultipleCompaniesAsync(List<Company> companies)
        {
            await _context.Companies.AddRangeAsync(companies);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompany(int id, Company company)
        {
            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    throw new CompanyNotFoundException();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task UpdateCompanyData(Company company)
        {
            var quote = await _yahooFinanceService.GetQuoteAsync(company.Symbol);

            if (quote == null)
            {
                throw new ArgumentException();
            }

            company.LastUpdated = DateTime.Now;

            _context.Entry(company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                throw new CompanyNotFoundException();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        public async Task<Company> FindAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task<Company?> FindWithEodDataAsync(int id)
        {
            return await _context.Companies.Include(c => c.EodData).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

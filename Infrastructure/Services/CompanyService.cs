using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Infrastructure.Models;
using GptFinance.Domain.Aggregate;
using GptFinance.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GptFinance.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        //private readonly IYahooFinanceService<CsvRecord> _yahooFinanceService;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(IYahooFinanceService<CsvRecord> yahooFinanceService, ICompanyRepository companyRepository)
        {
            //_yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public async Task<ICollection<CompanyAggregate>> GetAll(int take = 1000)
        {
            // Fetch the stock exchanges for each company.
            return await _companyRepository.GetAllAsync(take);
        }

        public async Task<CompanyAggregate> AddCompanyAsync(YahooSearchResult searchResult)
        {
            var company = new CompanyAggregate
            {
                Symbol = searchResult.Symbol,
                Name = searchResult.CompanyName,
                LastUpdate = DateTime.UtcNow
            };
            await _companyRepository.AddAsync(company);

            return company;
        }

        public async Task AddMultipleCompaniesAsync(List<CompanyAggregate> companies) => await _companyRepository.AddRange(companies);

        public async Task UpdateCompany(Guid id, CompanyAggregate company)
        {
            try
            {
                await _companyRepository.UpdateAsync(company);
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

        public async Task DeleteCompany(Guid id)
        {
            await _companyRepository.DeleteAsync(id);
        }

        private bool CompanyExists(Guid id) => _companyRepository.Exists(id);

        public async Task<CompanyAggregate> FindAsync(Guid id) => await _companyRepository.GetByIdAsync(id);

        public async Task<CompanyAggregate?> FindWithEodDataAsync(Guid id) => await _companyRepository.FindWithEodDataAsync(id);

        public async Task<ICollection<UpdateStatusAggregate>> GetCompanyUpdateStatus()
        {
            var companies = await _companyRepository.GetAllAsync(100);
            var latest = companies.Select(o => new UpdateStatusAggregate
            {
                EodLatestDateTime = o.FinancialData.EodData.First().Date,
                EodCount = o.FinancialData.EodData.Count()
            }).ToList();
            return latest;
        }
    }
}

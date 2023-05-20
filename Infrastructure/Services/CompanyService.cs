﻿using GptFinance.Application.Interfaces;
using GptFinance.Application.Models;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Models;
using GptFinance.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        //private readonly IYahooFinanceService<CsvRecord> _yahooFinanceService;
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(AppDbContext context, IYahooFinanceService<CsvRecord> yahooFinanceService, ICompanyRepository companyRepository)
        {
            //_yahooFinanceService = yahooFinanceService ?? throw new ArgumentNullException(nameof(yahooFinanceService));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        public async Task<ICollection<Company>> GetAll() => await _companyRepository.GetAllAsync();

        public async Task<Company> AddCompanyAsync(YahooSearchResult searchResult)
        {
            var company = new Company
            {
                Symbol = searchResult.Symbol,
                Name = searchResult.CompanyName,
                LastUpdated = DateTime.UtcNow
            };
            await _companyRepository.AddAsync(company);

            return company;
        }

        public async Task AddMultipleCompaniesAsync(List<Company> companies) => await _companyRepository.AddRange(companies);

        public async Task UpdateCompany(int id, Company company)
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

        /*
        public async Task UpdateCompanyData(Company company)
        {
            var quote = await _yahooFinanceService.GetQuoteAsync(company.Symbol);

            if (quote == null)
            {
                throw new ArgumentException();
            }

            company.LastUpdated = DateTime.Now;
            await _companyRepository.UpdateAsync(company);
        }
        */

        public async Task DeleteCompany(int id)
        {
            await _companyRepository.DeleteAsync(id);
        }

        private bool CompanyExists(int id) => _companyRepository.Exists(id);

        public async Task<Company> FindAsync(int id) => await _companyRepository.GetByIdAsync(id);

        public async Task<Company?> FindWithEodDataAsync(int id) => await _companyRepository.FindWithEodDataAsync(id);
    }
}

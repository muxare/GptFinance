using GptFinance.Application.Interfaces;
using GptFinance.Domain.Aggregate;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyAggregate> GetByIdAsync(Guid id)
    {
        var company = await _context.Companies.Include(o => o.EodData).FirstAsync(o => o.Id == id);
        var emas = await _context.EmaData.OrderByDescending(o => o.Date).ThenByDescending(o => o.Value).Take(12).ToListAsync();
        var macds = await _context.MacdData.OrderByDescending(o => o.Date).Take(12).ToListAsync();
        var result = new CompanyAggregate
        {
            Id = company.Id,
            Name = company.Name,
            Symbol = company.Symbol,
            FinancialData = new FinancialDataAggregate
            {
                EodData = company.EodData.Select(eod => eod.Map()).OrderByDescending(o => o.Date).Take(12).ToList(),
                EmaData = emas.Select(s => s.Map()).ToList(),
                MacdData = macds.Select(s => s.Map()).ToList()
            }
        };
        return result;
    }

    public async Task<ICollection<CompanyAggregate>> GetAllAsync(int take = 1000)
    {
        var companies = await _context.Companies.OrderBy(o => o.Id)
            .Include(c => c.EodData.OrderByDescending(o => o.Date).Take(take))
            .Include(c => c.StockExchange).ToListAsync();
        var emas = await _context.EmaData.GroupBy(e => e.CompanyId).OrderBy(o => o.Key).Select(e => e.OrderByDescending(o => o.Date).ThenByDescending(o => o.Value).Take(take)).ToListAsync();
        var macds = await _context.MacdData.GroupBy(e => e.CompanyId).OrderBy(o => o.Key).Select(e => e.OrderByDescending(o => o.Date).Take(take)).ToListAsync();
        var markets = await _context.StockExchange.ToListAsync();

        var companyAggregates = companies.Zip(emas, macds).Select(o => new CompanyAggregate
        {
            Id = o.First.Id,
            Name = o.First.Name,
            Symbol = o.First.Symbol,
            FinancialData = new FinancialDataAggregate
            {
                EodData = o.First.EodData.Select(eod => eod.Map()).OrderByDescending(o => o.Date).Take(12).ToList(),
                EmaData = o.Second.Select(s => s.Map()).ToList(),
                MacdData = o.Third.Select(s => s.Map()).ToList()
            },
            StockExchange = o.First.StockExchange.Map()
        }).ToList();

        return companyAggregates;
    }

    public async Task<CompanyAggregate> AddAsync(CompanyAggregate entity)
    {
        var result = await _context.Companies.AddAsync(entity.Map());
        await _context.SaveChangesAsync();
        return result.Entity.Map();
    }

    public async Task AddRange(ICollection<CompanyAggregate> entities)
    {
        await _context.Companies.AddRangeAsync(entities.Select(o => o.Map()));
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CompanyAggregate entity)
    {
        _context.Entry(entity.Map()).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company != null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
    }

    public bool Exists(Guid id)
    {
        return _context.Set<Company>().Any(c => c.Id == id);
    }

    public async Task<int> DeleteByIdAsync(Guid id)
    {
        if (!Exists(id))
            throw new Exception($"Company with id {id} not found");

        _context.Companies.Remove(new Company { Id = id });
        return await _context.SaveChangesAsync();
    }

    public async Task<CompanyAggregate?> FindWithEodDataAsync(Guid id)
    {
        return (await _context.Companies.Include(c => c.EodData).FirstOrDefaultAsync(c => c.Id == id)).Map();
    }
}

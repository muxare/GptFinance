using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Company> GetByIdAsync(Guid id) => await _context.Companies.FindAsync(id);

    public async Task<ICollection<Company>> GetAllAsync() => await _context.Companies.ToListAsync();

    public async Task<Company> AddAsync(Company entity)
    {
        var result = await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task AddRange(ICollection<Company> entities)
    {
        await _context.Companies.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
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

    public async Task<Company?> FindWithEodDataAsync(Guid id)
    {
        return await _context.Companies.Include(c => c.EodData).FirstOrDefaultAsync(c => c.Id == id);
    }
}
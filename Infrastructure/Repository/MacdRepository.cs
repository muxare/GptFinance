using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class MacdRepository : IMacdRepository
{
    private readonly AppDbContext _context;

    public MacdRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MacdData> GetByCompanyIdAndDateAsync(int companyId, DateTime date)
    {
        return await _context.MacdData.FirstOrDefaultAsync(m => m.CompanyId == companyId && m.Date == date);
    }

    public async Task<List<MacdData>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.MacdData.Where(m => m.CompanyId == companyId).ToListAsync();
    }

    public async Task<MacdData> AddAsync(MacdData entity)
    {
        _context.MacdData.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<MacdData> entities)
    {
        _context.MacdData.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int companyId, DateTime date)
    {
        return await _context.MacdData.AnyAsync(m => m.CompanyId == companyId && m.Date == date);
    }
}
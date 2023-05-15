using System.Linq.Expressions;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class EmaRepository : IEmaRepository
{
    private readonly AppDbContext _context;

    public EmaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmaData> GetByCompanyIdAndDateAsync(int companyId, DateTime date)
    {
        return await _context.EmaData.FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Date == date);
    }

    public async Task<List<EmaData>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.EmaData.Where(e => e.CompanyId == companyId).ToListAsync();
    }

    public async Task<EmaData> AddAsync(EmaData entity)
    {
        _context.EmaData.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<EmaData> entities)
    {
        _context.EmaData.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int companyId, DateTime date)
    {
        return await _context.EmaData.AnyAsync(e => e.CompanyId == companyId && e.Date == date);
    }

    public async Task<List<EmaData>> GetAsync(Expression<Func<EmaData, bool>> filter)
    {
        return await _context.EmaData.Where(filter).ToListAsync();
    }
}
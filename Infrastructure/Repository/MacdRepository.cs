using System.Linq.Expressions;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class MacdRepository : IMacdRepository
{
    private readonly AppDbContext _context;

    public MacdRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MacdDomainEntity> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date)
    {
        return (await _context.MacdData.FirstOrDefaultAsync(m => m.CompanyId == companyId && m.Date == date)).Map();
    }

    public async Task<List<MacdDomainEntity>> GetByCompanyIdAsync(Guid companyId)
    {
        return (await _context.MacdData.Where(m => m.CompanyId == companyId).ToListAsync()).Select(o => o.Map()).ToList();
    }

    public async Task<MacdDomainEntity> AddAsync(MacdDomainEntity entity)
    {
        _context.MacdData.Add(entity.Map());
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<MacdDomainEntity> entities)
    {
        _context.MacdData.AddRange(entities.Select(o => o.Map()));
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid companyId, DateTime date)
    {
        return await _context.MacdData.AnyAsync(m => m.CompanyId == companyId && m.Date == date);
    }

    public async Task<List<MacdDomainEntity>> GetAsync(Expression<Func<MacdDomainEntity, bool>> filter)
    {
        return (await _context.MacdData.Where(MapperExtentions.ConvertExpression(filter)).ToListAsync()).Select(o => o.Map()).ToList();
    }
}

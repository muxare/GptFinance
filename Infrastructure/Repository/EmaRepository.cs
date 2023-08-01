using System.Linq.Expressions;
using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class EmaRepository : IEmaRepository
{
    private readonly AppDbContext _context;

    public EmaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ema> GetByCompanyIdAndDateAsync(Guid companyId, DateTime date)
    {
        var emaData = await _context.EmaData.FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Date == date);
        return emaData.Map();
    }

    public async Task<List<Ema>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.EmaData.Where(e => e.CompanyId == companyId).Select(ema => ema.Map()).ToListAsync();
    }

    public async Task<Ema> AddAsync(Ema entity)
    {
        _context.EmaData.Add(entity.Map());
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<Ema> entities)
    {
        _context.EmaData.AddRange(entities.Select(ema => ema.Map()));
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid companyId, DateTime date)
    {
        return await _context.EmaData.AnyAsync(e => e.CompanyId == companyId && e.Date == date);
    }

    public async Task<List<Ema>> GetAsync(Expression<Func<Ema, bool>> filter)
    {
        return await _context.EmaData.Where(filter).ToListAsync();
    }

    // ToDo: Does this method belong here?
    public async Task<IDictionary<Guid, DateTime>> GetLastEodByCompany()
    {
        var t =
            from p in _context.Set<EmaData>()
            group p by p.CompanyId
            into p2
            select new
            {
                companyId = p2.Key,
                latest = p2.Max(e => e.Date)
            };

        return await t.ToDictionaryAsync(o => o.companyId, o => o.latest);
    }
}

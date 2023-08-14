using GptFinance.Application.Interfaces;
using GptFinance.Domain.Aggregate;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using GptFinance.Infrastructure.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class EodDataRepository : IEodDataRepository
{
    private readonly AppDbContext _context;

    public EodDataRepository(AppDbContext context)
    {
        _context = context;
    }

    public ValueTask<EodDomainEntity?> GetByIdAsync(Guid id) => _context.EodData.FindAsync(id).Map();

    public Task<List<EodDomainEntity>> GetAllAsync() => _context.EodData.Select(o => o.Map()).ToListAsync();

    public async Task<EodDomainEntity> AddAsync(EodDomainEntity entity)
    {
        var result = await _context.EodData.AddAsync(entity.Map());
        await SaveChangesAsync();
        return result.Entity.Map();
    }

    public async Task AddRange(ICollection<EodDomainEntity> entities)
    {
        await _context.EodData.AddRangeAsync(entities.Select(e => e.Map()));
        await SaveChangesAsync();
    }

    public async Task UpdateRageAsync(CompanyAggregate company)
    {
        var entities = company.FinancialData.EodData;
        var startDate = entities.MinBy(data => data.Date)?.Date;
        var endDate = entities.MaxBy(data => data.Date)?.Date;

        var entitiesInDb = _context.EodData
            .Where(data => data.CompanyId == company.Id && data.Date >= startDate && data.Date < endDate).ToList();

        // Need to match on company and date
        foreach (var entity in entities)
        {
            var match = entitiesInDb.FirstOrDefault(dbEntity => dbEntity.CompanyId == company.Id && dbEntity.Date.Date == entity.Date.Date);
            if (match != null)
            {
                match.Open = entity.Open;
                match.High = entity.High;
                match.Low = entity.Low;
                match.Close = entity.Close;
                match.Volume = entity.Volume;
            }
            else
            {
                var entityDb = entity.Map();
                entityDb.Id = Guid.NewGuid();
                entityDb.CompanyId = company.Id;
                _context.EodData.Add(entityDb);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EodDomainEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var eodData = await _context.EodData.FindAsync(id);
        if (eodData != null)
        {
            _context.EodData.Remove(eodData);
            await _context.SaveChangesAsync();
        }
    }

    public bool Exists(Guid id) => _context.Set<EodData>().Any(c => c.Id == id || c.CompanyId == id);

    public async Task<int> DeleteByIdAsync(Guid id)
    {
        var eodData = await _context.EodData.FindAsync(id);
        if (eodData == null)
            throw new Exception($"EodData with id {id} not found");

        _context.EodData.Remove(eodData);
        return await _context.SaveChangesAsync();
    }

    public Task<List<EodDomainEntity>> GetQuotesByCompanyId(Guid id) =>
        _context.EodData.Where(e => e.CompanyId == id).OrderBy(o => o.Date).Select(o => o.Map()).ToListAsync();

    public async Task<int> DeleteByCompanyId(Guid id)
    {
        var dataToBeRemoved = _context.EodData.Where(eod => eod.CompanyId == id).ToList();
        _context.EodData.RemoveRange(dataToBeRemoved);
        return await _context.SaveChangesAsync();
    }

    public async Task UpdateDataByCompanyId(int companyId, ICollection<EodDomainEntity> eodData)
    {
        _context.EodData.RemoveRange(eodData.Select(o => o.Map()));
        await _context.EodData.AddRangeAsync(eodData.Select(o => o.Map()));
        await _context.SaveChangesAsync();
    }

    public async Task<IDictionary<Guid, EodDomainEntity>> GetLastEods()
    {
        var r = await _context.EodData
            .GroupBy(e => e.CompanyId)
            .Select(g => new { g.Key, Value = g.OrderByDescending(e => e.Date).FirstOrDefault().Map() })
            .ToDictionaryAsync(o => o.Key, o => o.Value);

        return r;
    }

    private Task SaveChangesAsync() => _context.SaveChangesAsync();
}

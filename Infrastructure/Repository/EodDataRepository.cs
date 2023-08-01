using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entity;
using GptFinance.Infrastructure.Data;
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

    public ValueTask<Eod?> GetByIdAsync(Guid id) => _context.EodData.FindAsync(id);

    public Task<List<Eod>> GetAllAsync() => _context.EodData.ToListAsync();

    public async Task<Eod> AddAsync(Eod entity)
    {
        var result = await _context.EodData.AddAsync(entity.Map());
        await SaveChangesAsync();
        return result.Entity;
    }

    public async Task AddRange(ICollection<Eod> entities)
    {
        await _context.EodData.AddRangeAsync(entities.Select(e => e.Map()));
        await SaveChangesAsync();
    }

    public async Task UpdateRageAsync(ICollection<EodData> entities)
    {
        var startDate = entities.MinBy(data => data.Date)?.Date;
        var endDate = entities.MaxBy(data => data.Date)?.Date;

        var entitiesInDb = _context.EodData
            .Where(data => data.CompanyId == entities.First().CompanyId && data.Date >= startDate && data.Date < endDate).ToList();

        // Need to match on company and date
        foreach (var entity in entities)
        {
            var match = entitiesInDb.FirstOrDefault(dbEntity => dbEntity.CompanyId == entity.CompanyId && dbEntity.Date.Date == entity.Date.Date);
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
                _context.EodData.Add(entity);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EodData entity)
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

    public Task<List<EodData>> GetQuotesByCompanyId(Guid id) =>
        _context.EodData.Where(e => e.CompanyId == id).OrderBy(o => o.Date).ToListAsync();

    public async Task<int> DeleteByCompanyId(Guid id)
    {
        var dataToBeRemoved = _context.EodData.Where(eod => eod.CompanyId == id).ToList();
        _context.EodData.RemoveRange(dataToBeRemoved);
        return await _context.SaveChangesAsync();
    }

    public async Task UpdateDataByCompanyId(int companyId, ICollection<EodData> eodData)
    {
        _context.EodData.RemoveRange(eodData);
        await _context.EodData.AddRangeAsync(eodData);
        await _context.SaveChangesAsync();
    }

    public async Task<IDictionary<Guid, EodData>> GetLastEods()
    {
        var r = await _context.EodData
            .GroupBy(e => e.CompanyId)
            .Select(g => g.OrderByDescending(e => e.Date).FirstOrDefault())
            .ToDictionaryAsync(o => o.CompanyId, o => o);

        return r;
    }

    private Task SaveChangesAsync() => _context.SaveChangesAsync();
}

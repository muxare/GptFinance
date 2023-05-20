using GptFinance.Application.Interfaces;
using GptFinance.Domain.Entities;
using GptFinance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

public class EodDataRepository : IEodDataRepository
{
    private readonly AppDbContext _context;

    public EodDataRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EodData> GetByIdAsync(int id) => await _context.EodData.FindAsync(id);

    public async Task<ICollection<EodData>> GetAllAsync() => await _context.EodData.ToListAsync();

    public async Task<EodData> AddAsync(EodData entity)
    {
        var result = await _context.EodData.AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task AddRange(ICollection<EodData> entities)
    {
        await _context.EodData.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EodData entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var eodData = await _context.EodData.FindAsync(id);
        if (eodData != null)
        {
            _context.EodData.Remove(eodData);
            await _context.SaveChangesAsync();
        }
    }
    public bool Exists(int id)
    {
        return _context.Set<EodData>().Any(c => c.Id == id && c.CompanyId == id);
    }

    public async Task<int> DeleteByIdAsync(int id)
    {

        var eodData = await _context.EodData.FindAsync(id);
        if (eodData == null)
            throw new Exception($"EodData with id {id} not found");

        _context.EodData.Remove(eodData);
        return await _context.SaveChangesAsync();
    }

    public async Task<ICollection<EodData>> GetQuotesByCompanyId(int id)
    {
        var eodData = await _context.EodData.Where(e => e.CompanyId == id).OrderBy(o => o.Date).ToListAsync();
        return eodData;
    }

    public async Task<int> DeleteByCompanyId(int id)
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
}
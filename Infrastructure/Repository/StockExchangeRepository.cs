using GptFinance.Application.Interfaces;
using GptFinance.Domain.Aggregate;
using GptFinance.Infrastructure.Data;
using GptFinance.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace GptFinance.Infrastructure.Repository;

//public class StockExchangeRepository : IStockExchangeRepository
//{
//    private readonly AppDbContext _context;

//    public StockExchangeRepository(AppDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<IEnumerable<StockExchangeAggregate>> GetAllAsync()
//    {
//        return (await _context.StockExchange.ToListAsync()).Select(o => o.Map());
//    }

//    public async Task<StockExchangeAggregate> GetByIdAsync(int id)
//    {
//        return (await _context.StockExchange.FindAsync(id)).Map();
//    }

//    public async Task AddAsync(StockExchangeAggregate stockExchange)
//    {
//        await _context.StockExchange.AddAsync(stockExchange.Map());
//    }

//    public Task UpdateAsync(StockExchangeAggregate stockExchange)
//    {
//        _context.StockExchange.Update(stockExchange.Map());
//        return Task.CompletedTask;
//    }

//    public async Task DeleteAsync(int id)
//    {
//        var stockExchange = await _context.StockExchange.FindAsync(id);
//        if (stockExchange != null)
//        {
//            _context.StockExchange.Remove(stockExchange);
//        }
//    }

//    public async Task SaveChangesAsync()
//    {
//        await _context.SaveChangesAsync();
//    }
//}

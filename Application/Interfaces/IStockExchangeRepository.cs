using GptFinance.Domain.Aggregate;

namespace GptFinance.Application.Interfaces;

public interface IStockExchangeRepository
{
    Task<IEnumerable<StockExchangeAggregate>> GetAllAsync();

    Task<StockExchangeAggregate> GetByIdAsync(int id);

    Task AddAsync(StockExchangeAggregate stockExchange);

    Task UpdateAsync(StockExchangeAggregate stockExchange);

    Task DeleteAsync(int id);

    Task SaveChangesAsync();
}

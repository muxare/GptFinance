namespace GptFinance.Infrastructure.Repository;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<ICollection<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task AddRange(ICollection<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);

    bool Exists(int id);
}

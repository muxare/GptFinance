using System.Linq.Expressions;

namespace GptFinance.Application.Interfaces;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<ICollection<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task AddRange(ICollection<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    bool Exists(int id);
    Task<int> DeleteByIdAsync(int id);
}
using System.Linq.Expressions;

namespace HiveHQ.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    
    Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>> predicate);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveChangesAsync();
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

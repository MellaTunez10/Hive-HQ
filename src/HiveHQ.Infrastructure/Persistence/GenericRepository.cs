using HiveHQ.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HiveHQ.Infrastructure.Persistence;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public void Update(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
    public async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate != null)
        {
            return await _context.Set<T>().CountAsync(predicate);
        }
        return await _context.Set<T>().CountAsync();
    }
    public async Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>()
            .Where(predicate)
            .ToListAsync();
    }

}

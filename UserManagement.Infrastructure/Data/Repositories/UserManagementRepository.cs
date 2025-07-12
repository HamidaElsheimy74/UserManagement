using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Data.Repositories;
public class UserManagementRepository<T> : IUserManagementRepository<T> where T : class
{

    private readonly AppDbContext _context;
    public UserManagementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().AnyAsync(expression);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;
        var deleted = _context.Set<T>().Remove(entity);
        return deleted.State == EntityState.Deleted ? true : false;
    }

    public async Task<T> GetByIdAsync(long id) => await _context.Set<T>().FindAsync(id);

    public async Task<List<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<List<T>> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().Where(predicate);
        foreach (var item in includes)
        {
            query = query.Include(item);
        }
        return await query.ToListAsync();
    }

    public async Task<T> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().Where(predicate);
        foreach (var item in includes)
        {
            query = query.Include(item);
        }
        return await query.FirstOrDefaultAsync();
    }
}

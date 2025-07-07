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

    public Task<bool> DeleteAsync(long id)
    {
        var entity = GetByIdAsync(id);
        if (entity == null)
            return Task.FromResult(false);
        var deleted = _context.Set<T>().Remove(entity.Result);
        return deleted.State == EntityState.Deleted ? Task.FromResult(true) : Task.FromResult(false);
    }

    public async Task<T> GetByIdAsync(long id) => await _context.Set<T>().FindAsync(id);


    public async Task<IReadOnlyList<T>> ListAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

}

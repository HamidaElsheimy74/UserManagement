using System.Linq.Expressions;

namespace UserManagement.Domain.Interfaces;
public interface IUserManagementRepository<T> where T : class
{
    Task<T> GetByIdAsync(long id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(long id);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    Task<List<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includes);
    Task<List<T>> Where(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

}

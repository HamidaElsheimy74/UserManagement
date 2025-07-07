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
}

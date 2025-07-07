using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Data.Repositories;
public class UserManagementUnitOfWork : IUserManagementUnitOfWork
{
    private readonly AppDbContext _context;
    private Dictionary<Type, object> _repositories;
    public UserManagementUnitOfWork(AppDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public IUserManagementRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (IUserManagementRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        var repository = new UserManagementRepository<TEntity>(_context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public async Task<int> Save() => await _context.SaveChangesAsync();
}

namespace UserManagement.Domain.Interfaces;
public interface IUserManagementUnitOfWork : IDisposable
{
    IUserManagementRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> Save();
}

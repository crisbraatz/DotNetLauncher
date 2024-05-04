namespace Infrastructure.PostgreSql;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
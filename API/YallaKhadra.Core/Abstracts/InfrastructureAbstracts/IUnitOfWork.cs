using Microsoft.EntityFrameworkCore.Storage;

namespace YallaKhadra.Core.Abstracts.InfrastructureAbstracts;

public interface IUnitOfWork : IAsyncDisposable {
    //IGenericRepository<ApplicationUser> Users { get; }

    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

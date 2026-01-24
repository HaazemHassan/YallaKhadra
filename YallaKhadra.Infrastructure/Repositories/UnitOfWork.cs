using Microsoft.EntityFrameworkCore.Storage;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork {
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;


    public UnitOfWork(AppDbContext context) {
        _context = context;
    }

    public async Task<int> SaveChangesAsync() {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken) {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitAsync() {
        if (_transaction != null)
            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync() {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }

    public async ValueTask DisposeAsync() {
        if (_transaction != null)
            await _transaction.DisposeAsync();

        await _context.DisposeAsync();
    }
}

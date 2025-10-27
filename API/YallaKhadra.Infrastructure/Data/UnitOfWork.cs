﻿using Microsoft.EntityFrameworkCore.Storage;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;

namespace YallaKhadra.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork {
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    //public IGenericRepository<User> Users { get; }

    public UnitOfWork(AppDbContext context) {
        _context = context;

        //Users = new GenericRepository<User>(_context);
    }

    public async Task<int> SaveChangesAsync() {
        return await _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() {
        _transaction = await _context.Database.BeginTransactionAsync();
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

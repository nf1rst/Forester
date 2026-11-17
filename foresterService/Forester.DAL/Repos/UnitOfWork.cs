using System;
using Forester.DAL.Data;
using Forester.Domain.Interfaces;

namespace Forester.DAL.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly ForesterDBContext context;
    public ILogRepository Logs { get; }

    public UnitOfWork(
        ForesterDBContext context,
        ILogRepository logs
    )
    {
        this.context = context;
        this.Logs = logs;
    }
    public void Dispose()
    {
        context.Dispose();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}

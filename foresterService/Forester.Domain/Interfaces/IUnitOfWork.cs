using System;

namespace Forester.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public ILogRepository Logs { get; }

    Task<int> SaveChangesAsync();
}

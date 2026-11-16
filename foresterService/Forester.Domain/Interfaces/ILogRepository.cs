using System;
using Forester.Domain.Entities;

namespace Forester.Domain.Interfaces;

public interface ILogRepository
{
    Task InsertLogAsync(Log log);
    Task<ICollection<Log>> GetAllLogsByUserUsername(string userUsername);
}

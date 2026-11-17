using System;
using Forester.DAL.Data;
using Forester.Domain.Entities;
using Forester.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forester.DAL.Repos;

public class LogRepository : ILogRepository
{
    private readonly ForesterDBContext context;
    public LogRepository(ForesterDBContext context)
    {
        this.context = context;
    }
    public async Task<ICollection<Log>> GetAllLogsByUserUsername(string userUsername)
    {
        return await context.Logs.Where(l => l.UserUsername == userUsername).ToListAsync();
    }

    public async Task InsertLogAsync(Log log)
    {
        await context.Logs.AddAsync(log);
    }
}

using System;
using Forester.DAL.Data;
using Forester.DAL.Repos;
using Forester.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forester.DAL.Extensions;

public static class DALServiceExtension
{
    public static IServiceCollection SetDALServices(this IServiceCollection service, string connectionString)
    {
        service.AddDbContext<ForesterDBContext>(opt => opt.UseSqlite(connectionString));

        service.AddScoped<ILogRepository, LogRepository>();
        service.AddScoped<IUnitOfWork, UnitOfWork>();

        return service;
    }

}

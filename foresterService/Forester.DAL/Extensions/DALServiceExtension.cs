using System;
using Forester.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forester.DAL.Extensions;

public static class DALServiceExtension
{
    public static IServiceCollection SetDALServices(this IServiceCollection service, string connectionString)
    {
        service.AddDbContext<ForesterDBContext>(opt => opt.UseSqlite(connectionString));

        return service;
    }

}

using System;
using Forester.Application.Interfaces;
using Forester.Application.Mapper;
using Forester.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Forester.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection SetApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        services.AddScoped<ILogService, LogService>();
        return services;
    }
}

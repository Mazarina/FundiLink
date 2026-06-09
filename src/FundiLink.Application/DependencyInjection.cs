using FluentValidation;
using FundiLink.Application.Features.AcademicProfile.Services;
using FundiLink.Application.Features.ProgrammeMatching.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FundiLink.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ApsCalculatorService>();
        services.AddScoped<ProgrammeMatchingService>();

        return services;
    }
}

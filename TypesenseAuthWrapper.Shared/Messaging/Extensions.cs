using System.Reflection;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace TypesenseAuthWrapper.Shared.Messaging;

internal static class Extensions
{
    internal static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        var assembliesWithConsumers = GetAssembliesWithConsumers();
        services.AddMassTransit(busConfigurator =>
        {
            if (assembliesWithConsumers.Any())
            {
                busConfigurator.AddConsumers(assembliesWithConsumers);
            }

            busConfigurator.UsingInMemory((context, inMemoryConfigurator) =>
            {
                inMemoryConfigurator.ConfigureEndpoints(context);
            });
        });
        return services;
    }

    private static Assembly[] GetAssembliesWithConsumers()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                        && p is { IsClass: true, IsAbstract: false })
            .Select(p => p.Assembly)
            .Distinct()
            .Where(a => a.FullName?.Contains("MassTransit") == false)
            .ToArray();
        return assemblies;
    }
}
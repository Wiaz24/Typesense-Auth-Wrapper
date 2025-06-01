namespace TypesenseAuthWrapper.Bootstrapper.TypesenseMiddleware;

internal static class Extensions
{
    private const string TypesenseSectionName = "Typesense";
    
    internal static IServiceCollection AddTypesense(this IServiceCollection services)
    {
        services.AddOptions<TypesenseOptions>()
            .BindConfiguration(TypesenseSectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddTransient<TypesenseMiddleware>();
        return services;
    }
}
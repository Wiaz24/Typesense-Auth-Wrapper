namespace TypesenseAuthWrapper.Bootstrapper.Cors;

public static class Extensions
{
    private const string PolicyName = "Frontend";
    private const string SectionName = "AllowedOrigins";

    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedHosts = configuration.GetSection(SectionName).Get<string[]>()
                           ?? throw new NullReferenceException($"{SectionName} not found in config file");

        services.AddCors(
            options => options.AddPolicy(
                PolicyName,
                policy => policy
                    .WithOrigins(allowedHosts)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
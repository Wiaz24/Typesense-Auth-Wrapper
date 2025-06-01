using TypesenseAuthWrapper.Bootstrapper.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TypesenseAuthWrapper.Bootstrapper.Auth;

internal static class Extensions
{
    private const string OidcSectionName = "Oidc";

    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OidcOptions>()
            .BindConfiguration(OidcSectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // var oidcOptions = services.BuildServiceProvider().GetRequiredService<IOptions<OidcOptions>>().Value;
                var oidcOptions = configuration.GetSection(OidcSectionName).Get<OidcOptions>()
                                  ?? throw new NullReferenceException($"{OidcSectionName} not found in config file");
                Console.WriteLine($"OIDC ClientId: {oidcOptions.ClientId}");
                Console.WriteLine($"OIDC MetadataAddress: {oidcOptions.MetadataAddress}");
                options.Authority = oidcOptions.MetadataAddress;
                options.RequireHttpsMetadata = oidcOptions.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = oidcOptions.ValidateIssuer,
                    ValidateAudience = oidcOptions.ValidateAudience,
                    ValidateLifetime = oidcOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = oidcOptions.ValidateIssuerSigningKey,
                    RoleClaimType = oidcOptions.RoleClaimType
                };
                options.MetadataAddress = oidcOptions.MetadataAddress;
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
            .AddPolicy("User", policy => policy.RequireRole("User"));

        return services;
    }

    internal static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.Map("/api/typesenseauthwrapper/oidc-config", appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var oidcOptions = app.ApplicationServices.GetRequiredService<IOptions<OidcOptions>>().Value;
                var response = new
                {
                    metadataUrl = oidcOptions.MetadataAddress,
                    clientId = oidcOptions.ClientId,
                };
                await context.Response.WriteAsJsonAsync(response);
            });
        });
        return app;
    }
}
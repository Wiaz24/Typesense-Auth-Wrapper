#define UseSwagger
using TypesenseAuthWrapper.Bootstrapper.Auth;
using TypesenseAuthWrapper.Bootstrapper.OpenApi.Swagger;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TypesenseAuthWrapper.Bootstrapper.OpenApi;

internal static class Extensions
{
    internal static OpenApiOptions UseOpenIdConnectAuthentication(this OpenApiOptions options,
        IConfiguration configuration)
    {
        var oidcOptions = configuration.GetSection("Oidc").Get<OidcOptions>()
                          ?? throw new NullReferenceException("Oidc options are missing in appsettings.json");
        var scheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.OpenIdConnect,
            OpenIdConnectUrl = new Uri(oidcOptions.MetadataAddress),
            Description = "OpenID Connect scheme",
            BearerFormat = "JWT",
            Extensions =
            {
                { "x-tokenName", new OpenApiString("id_token") }
            }
        };
        options.AddDocumentTransformer((document, context, token) =>
        {
            document.Components ??= new();
            document.Components.SecuritySchemes.Add("OIDC", scheme);
            return Task.CompletedTask;
        });
        return options;
    }

    internal static WebApplication MapCustomOpenApi(this WebApplication app)
    {
        app.MapOpenApi("/api/typesenseauthwrapper/openapi/{documentName}.json");
        app.MapGet("/", () => Results.Redirect("/api/typesenseauthwrapper/swagger"))
            .Produces(200)
            .ExcludeFromDescription();
        app.MapCustomSwagger();
        return app;
    }
}
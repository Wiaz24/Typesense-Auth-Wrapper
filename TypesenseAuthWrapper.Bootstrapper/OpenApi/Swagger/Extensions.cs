using TypesenseAuthWrapper.Bootstrapper.Auth;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace TypesenseAuthWrapper.Bootstrapper.OpenApi.Swagger;

internal static class Extensions
{
    internal static IApplicationBuilder MapCustomSwagger(this IApplicationBuilder app)
    {
        var oidcOptions = app.ApplicationServices.GetRequiredService<IOptions<OidcOptions>>().Value;

        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "TypesenseAuthWrapper";
            c.SwaggerEndpoint($"/api/typesenseauthwrapper/openapi/v1.json", "API V1");
            c.RoutePrefix = $"api/typesenseauthwrapper/swagger";

            c.OAuthClientId(oidcOptions.ClientId);
            c.OAuthClientSecret(oidcOptions.ClientSecret);
            c.OAuthUsePkce();
            c.InjectJavascript("logout.js");
            c.InjectStylesheet("https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css");
        });
        return app;
    }
}
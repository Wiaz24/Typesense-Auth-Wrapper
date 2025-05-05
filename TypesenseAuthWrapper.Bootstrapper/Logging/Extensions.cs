using Serilog;

namespace TypesenseAuthWrapper.Bootstrapper.Logging;

internal static class Extensions
{
    internal static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) => { loggerConfiguration.WriteTo.Console(); });
        return builder;
    }
}
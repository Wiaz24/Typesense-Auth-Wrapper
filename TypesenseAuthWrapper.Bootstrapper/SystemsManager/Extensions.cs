using Amazon.SimpleSystemsManagement;

namespace TypesenseAuthWrapper.Bootstrapper.SystemsManager;

internal static class Extensions
{
    internal static WebApplicationBuilder AddCustomSystemsManager(this WebApplicationBuilder builder)
    {
        var environmentName = builder.Environment.EnvironmentName;
        builder.Configuration.AddSystemsManager(configuration =>
        {
            configuration.AwsOptions = builder.Configuration.GetAWSOptions();
            configuration.Path = $"/TypesenseAuthWrapper/{environmentName}";
            configuration.ReloadAfter = TimeSpan.FromMinutes(5);
        });
        builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>();
        builder.Services.AddTransient<SsmHealthCheck>();
        builder.Services.AddHealthChecks()
            .AddCheck<SsmHealthCheck>("AWS Systems Manager");
        return builder;
    }
}
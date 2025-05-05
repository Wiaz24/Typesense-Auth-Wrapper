using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TypesenseAuthWrapper.Bootstrapper.SystemsManager;

public class SsmHealthCheck : IHealthCheck
{
    private readonly IAmazonSimpleSystemsManagement _ssmClient;

    public SsmHealthCheck(IAmazonSimpleSystemsManagement ssmClient)
    {
        _ssmClient = ssmClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var response = await _ssmClient.GetParameterAsync(new GetParameterRequest()
            {
                Name = "/TypesenseAuthWrapper/Development/Postgres/ConnectionString"
            }, cancellationToken);

            if (response?.Parameter != null)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
namespace Olympus.Api;

public class SelfHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
  public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
      Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
      CancellationToken cancellationToken = default)
  {
    // You can add more sophisticated checks here if needed for the API itself
    return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Olympus API is running."));
  }
}

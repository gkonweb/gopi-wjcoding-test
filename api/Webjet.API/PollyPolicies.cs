using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ILogger logger)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                2, TimeSpan.FromSeconds(30),
                onBreak: (outcome, timespan) =>
                {
                    logger.LogWarning("Circuit breaker opened due to {Reason}. Duration: {Duration}", outcome.Exception?.Message, timespan);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit breaker reset.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit breaker is half-open, next call is a trial.");
                });
    }
}

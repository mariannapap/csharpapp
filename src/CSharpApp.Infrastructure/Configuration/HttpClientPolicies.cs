using Polly;
using Polly.Extensions.Http;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpClientPolicies
{
	public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(HttpClientSettings httpClientSettings)
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.WaitAndRetryAsync(
				httpClientSettings.RetryCount,
				retryAttempt => TimeSpan.FromMilliseconds(httpClientSettings.SleepDuration * retryAttempt)
			);
	}
}

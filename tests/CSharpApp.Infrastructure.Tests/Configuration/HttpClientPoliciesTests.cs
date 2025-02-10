using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Configuration;

namespace CSharpApp.Infrastructure.Tests.Configuration;

public class HttpClientPoliciesTests
{
	[Fact]
	public void GetRetryPolicy_ShouldReturnPolicy_WhenConfigurationIsValid()
	{
		// Arrange
		var httpClientSettings = new HttpClientSettings
		{
			RetryCount = 3,
			SleepDuration = 100
		};

		// Act
		var policy = HttpClientPolicies.GetRetryPolicy(httpClientSettings);

		// Assert
		Assert.NotNull(policy);
	}

	[Fact]
	public async Task GetRetryPolicy_ShouldRetry_OnTransientError()
	{
		// Arrange
		var httpClientSettings = new HttpClientSettings
		{
			RetryCount = 3,
			SleepDuration = 100
		};

		var policy = HttpClientPolicies.GetRetryPolicy(httpClientSettings);

		int attemptCount = 0;

		// Act
		await policy.ExecuteAsync(async () =>
		{
			attemptCount++;
			if(attemptCount < 3)
				throw new HttpRequestException();
			return await Task.FromResult(new HttpResponseMessage());
		});

		// Assert
		Assert.Equal(3, attemptCount);
	}
}

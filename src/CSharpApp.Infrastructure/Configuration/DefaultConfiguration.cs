using CSharpApp.Application.Products.Queries.Handlers;
using CSharpApp.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
	public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
	{
		var serviceProvider = services.BuildServiceProvider();

		var configuration = serviceProvider.GetService<IConfiguration>()
			?? throw new ArgumentNullException(nameof(RestApiSettings), "Configuration cannot be null.");

		var restApiSettings = configuration.GetSection(nameof(RestApiSettings)).Get<RestApiSettings>() switch
		{
			{ BaseUrl.Length: > 0, Products.Length: > 0, Categories.Length: > 0, Auth.Length: > 0 } s => s,
			_ => throw new ArgumentNullException(nameof(RestApiSettings), $"Missing configuration from {nameof(RestApiSettings)}")
		};

		var httpClientSettings = configuration.GetSection(nameof(HttpClientSettings)).Get<HttpClientSettings>() switch
		{
			{ RetryCount: >= 0, SleepDuration: >= 0, LifeTime: > 0 } s => s,
			_ => throw new ArgumentNullException(nameof(RestApiSettings), $"Missing configuration from {nameof(HttpClientSettings)}")
		};

		services.AddHttpClient(
			restApiSettings.Products!,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
			}
		)
		.AddPolicyHandler(HttpClientPolicies.GetRetryPolicy(httpClientSettings))
		.SetHandlerLifetime(TimeSpan.FromMinutes(httpClientSettings.LifeTime));

		services.AddHttpClient(
			restApiSettings.Categories!,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
			}
		).AddPolicyHandler(HttpClientPolicies.GetRetryPolicy(httpClientSettings));

		services.AddHttpClient(
			restApiSettings.Auth!,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
			}
		).AddPolicyHandler(HttpClientPolicies.GetRetryPolicy(httpClientSettings));

		services.Configure<RestApiSettings>(configuration.GetSection(nameof(RestApiSettings)));
		services.AddScoped<IProductsService, ProductsService>();
		services.AddScoped<ICategoryService, CategoryService>();
		services.AddScoped<IAuthService, AuthService>();

		services.Configure<TokenCacheConfig>(configuration.GetSection(nameof(TokenCacheConfig)));
		services.AddScoped<ITokenCacheService, TokenCacheService>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetAllProductsQueryHandler).Assembly));

		return services;
	}
}

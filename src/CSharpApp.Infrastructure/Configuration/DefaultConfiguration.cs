using CSharpApp.Application.Products.Queries.Handlers;
using CSharpApp.Infrastructure.Data;

namespace CSharpApp.Infrastructure.Configuration;

public static class DefaultConfiguration
{
	public static IServiceCollection AddDefaultConfiguration(this IServiceCollection services)
	{
		var serviceProvider = services.BuildServiceProvider();

		var configuration = serviceProvider
			.GetService<IConfiguration>()
			?? throw new InvalidOperationException("Configuration cannot be null.");

		var restApiSettings = configuration
			.GetSection(nameof(RestApiSettings))
			.Get<RestApiSettings>()
			?? throw new InvalidOperationException("RestApiSettings are not configured properly.");

		if(string.IsNullOrEmpty(restApiSettings.BaseUrl))
			throw new InvalidOperationException("BaseUrl cannot be null or empty.");

		if(string.IsNullOrEmpty(restApiSettings.Products)
			|| string.IsNullOrEmpty(restApiSettings.Categories)
			|| string.IsNullOrEmpty(restApiSettings.Auth))
			throw new InvalidOperationException("HttpClient name cannot be null or empty.");

		services.AddHttpClient(
			restApiSettings.Products,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl);
			}
		);
		services.AddHttpClient(
			restApiSettings.Categories,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl);
			}
		);
		services.AddHttpClient(
			restApiSettings.Auth,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl);
			}
		);

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

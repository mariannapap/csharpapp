using CSharpApp.Application.Products.Queries.Handlers;

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

		if(string.IsNullOrEmpty(restApiSettings.Name))
			throw new InvalidOperationException("HttpClient Name cannot be null or empty.");

		services.Configure<RestApiSettings>(configuration.GetSection(nameof(RestApiSettings)));

		services.AddHttpClient(
			restApiSettings.Name,
			client =>
			{
				client.BaseAddress = new Uri(restApiSettings.BaseUrl);
			}
		);
		services.AddScoped<IProductsService, ProductsService>();
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetAllProductsQueryHandler).Assembly));

		return services;
	}
}

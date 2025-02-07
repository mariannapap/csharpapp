namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly RestApiSettings _restApiSettings;
	private readonly ILogger<ProductsService> _logger;

	public ProductsService(
		IHttpClientFactory httpClientFactory,
		IOptionsSnapshot<RestApiSettings> restApiSettings,
		ILogger<ProductsService> logger
	)
	{
		_httpClientFactory = httpClientFactory;
		_restApiSettings = restApiSettings.Value;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<Product?>> GetProducts()
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Name);
		var response = await client.GetAsync(_restApiSettings.Products);
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		var products = JsonSerializer.Deserialize<List<Product?>>(content) ?? [];

		return products.AsReadOnly();
	}
}
using CSharpApp.Core.Dtos.Requests;
using CSharpApp.Infrastructure.Extensions;

namespace CSharpApp.Infrastructure.Data;

public class ProductsService(
	IHttpClientFactory httpClientFactory,
	IOptionsSnapshot<RestApiSettings> restApiSettings
) : IProductsService
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly RestApiSettings _restApiSettings = restApiSettings.Value;

	public async Task<Product?> GetProductById(int id, CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Products!);
		var response = await client.GetAsync(_restApiSettings.Products + "/" + id, cancellationToken);
		var responseBody = await response.HandleResponse(cancellationToken);
		return JsonSerializer.Deserialize<Product?>(responseBody);
	}

	public async Task<IReadOnlyCollection<Product?>> GetProducts(CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Products!);
		var response = await client.GetAsync(_restApiSettings.Products, cancellationToken);
		var responseBody = await response.HandleResponse(cancellationToken);
		return (JsonSerializer.Deserialize<List<Product?>>(responseBody) ?? []).AsReadOnly();
	}

	public async Task<Product?> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Products!);
		var productJson = JsonSerializer.Serialize(request);
		var content = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
		var response = await client.PostAsync(_restApiSettings.Products, content, cancellationToken);

		var responseBody = await response.HandleResponse(cancellationToken);
		return JsonSerializer.Deserialize<Product>(responseBody);
	}
}
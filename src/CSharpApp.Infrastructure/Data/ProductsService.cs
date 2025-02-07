using CSharpApp.Application.Products.Commands;
using CSharpApp.Core.Dtos.Requests;
using CSharpApp.Core.Exceptions;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly RestApiSettings _restApiSettings;

	public ProductsService(
		IHttpClientFactory httpClientFactory,
		IOptionsSnapshot<RestApiSettings> restApiSettings
	)
	{
		_httpClientFactory = httpClientFactory;
		_restApiSettings = restApiSettings.Value;
	}

	public async Task<Product?> GetProductById(int id, CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Name);
		var response = await client.GetAsync(_restApiSettings.Products + "/" + id, cancellationToken);
		var content = await HandleResponse(response, cancellationToken);
		var product = JsonSerializer.Deserialize<Product?>(content);

		return product;
	}

	public async Task<IReadOnlyCollection<Product?>> GetProducts(CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Name);
		var response = await client.GetAsync(_restApiSettings.Products, cancellationToken);
		var content = await HandleResponse(response, cancellationToken);
		var products = JsonSerializer.Deserialize<List<Product?>>(content) ?? [];

		return products.AsReadOnly();
	}

	public async Task<Product?> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Name);
		var productJson = JsonSerializer.Serialize(request);
		var content = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
		var response = await client.PostAsync(_restApiSettings.Products, content, cancellationToken);

		var responseBody = await HandleResponse(response, cancellationToken);
		var product = JsonSerializer.Deserialize<Product>(responseBody);

		return product;
	}

	#region Private Methods

	private async Task<string> HandleResponse(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		var content = await response.Content.ReadAsStringAsync(cancellationToken);

		switch(response.StatusCode)
		{
			case System.Net.HttpStatusCode.NotFound:
				throw new NotFoundException(content);
			case System.Net.HttpStatusCode.BadRequest:
				throw new BadRequestException(content);
			case System.Net.HttpStatusCode.InternalServerError:
				throw new ServerErrorException(content);
			default:
				response.EnsureSuccessStatusCode();
				break;
		}

		return content;
	}

	#endregion
}
using CSharpApp.Infrastructure.Extensions;

namespace CSharpApp.Infrastructure.Data;

public class CategoryService(
	IHttpClientFactory httpClientFactory,
	IOptionsSnapshot<RestApiSettings> restApiSettings
) : ICategoryService
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly RestApiSettings _restApiSettings = restApiSettings.Value;

	public async Task<IReadOnlyCollection<Category?>> GetAllCategories(CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Categories!);
		var response = await client.GetAsync(_restApiSettings.Categories, cancellationToken);

		var responseBody = await response.HandleResponse(cancellationToken);
		return (JsonSerializer.Deserialize<List<Category?>>(responseBody) ?? []).AsReadOnly();
	}
}

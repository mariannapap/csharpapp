using CSharpApp.Core.Exceptions;

namespace CSharpApp.Infrastructure.Data;

public class CategoryService : ICategoryService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly RestApiSettings _restApiSettings;

	public CategoryService(
		IHttpClientFactory httpClientFactory,
		IOptionsSnapshot<RestApiSettings> restApiSettings
	)
	{
		_httpClientFactory = httpClientFactory;
		_restApiSettings = restApiSettings.Value;
	}

	public async Task<IReadOnlyCollection<Category?>> GetAllCategories(CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_restApiSettings.Categories!);
		var response = await client.GetAsync(_restApiSettings.Categories, cancellationToken);

		var content = await HandleResponse(response);
		var categories = JsonSerializer.Deserialize<List<Category?>>(content) ?? new List<Category?>();

		return categories.AsReadOnly();
	}

	private async Task<string> HandleResponse(HttpResponseMessage response)
	{
		var content = await response.Content.ReadAsStringAsync();

		if(response.IsSuccessStatusCode)
			return content;

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
				return content; // This line will never be reached, but is required to satisfy the return type.
		}
	}
}

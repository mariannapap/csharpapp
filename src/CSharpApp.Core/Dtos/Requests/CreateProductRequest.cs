namespace CSharpApp.Core.Dtos.Requests;

public class CreateProductRequest
{
	[JsonPropertyName("title")]
	public required string Title { get; init; }

	[JsonPropertyName("price")]
	public decimal Price { get; init; }

	[JsonPropertyName("description")]
	public required string Description { get; init; }

	[JsonPropertyName("images")]
	public List<string> Images { get; init; } = [];

	[JsonPropertyName("categoryId")]
	public int CategoryId { get; init; }
}

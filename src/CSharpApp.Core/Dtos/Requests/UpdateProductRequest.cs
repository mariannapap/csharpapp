namespace CSharpApp.Core.Dtos.Requests;

public class UpdateProductRequest
{
	[JsonPropertyName("title")]
	public string? Title{ get; init; }

	[JsonPropertyName("price")]
	public decimal? Price{ get; init; }

	[JsonPropertyName("images")]
	public List<string> Images { get; init; } = [];
}

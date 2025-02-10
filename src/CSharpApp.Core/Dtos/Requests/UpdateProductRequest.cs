namespace CSharpApp.Core.Dtos.Requests;

public class UpdateProductRequest
{
	public string? Title{ get; init; }
	public decimal? Price{ get; init; }
	public List<string> Images { get; init; } = [];
}

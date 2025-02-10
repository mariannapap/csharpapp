using System.Text.Json.Serialization;

namespace CSharpApp.Application.Products.Commands;

public class UpdateProductCommand : IRequest<int>
{
	public int Id { get; init; }
	public string? Title { get; init; }
	public decimal? Price { get; init; }
	public List<string> Images { get; init; } = [];
}

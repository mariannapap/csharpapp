namespace CSharpApp.Application.Products.Commands;

public class CreateProductCommand : IRequest<int>
{
	public required string Title { get; init; }
	public decimal Price { get; init; }
	public required string Description { get; init; }
	public int CategoryId { get; init; }
	public required List<string> Images { get; init; }
}

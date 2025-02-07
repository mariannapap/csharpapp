namespace CSharpApp.Application.Products.Commands;

public class CreateProductCommand : IRequest<int>
{
	public required string Title { get; set; }
	public decimal Price { get; set; }
	public required string Description { get; set; }
	public int CategoryId { get; set; }
	public required List<string> Images { get; set; }
}

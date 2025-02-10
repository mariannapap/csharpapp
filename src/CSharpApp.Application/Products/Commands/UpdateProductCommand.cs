namespace CSharpApp.Application.Products.Commands;

public class UpdateProductCommand : IRequest<int>
{
	public int Id { get; init; }
	public string? Name { get; init; }
	public decimal? Price { get; init; }
	public string? Description { get; init; }
}

namespace CSharpApp.Application.Products.Queries;

public class GetProductByIdQuery : IRequest<Product>
{
    public int Id { get; init; }
}

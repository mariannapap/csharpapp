namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    public Task<IReadOnlyCollection<Product?>> GetProducts(CancellationToken cancellationToken);

	public Task<Product?> GetProductById(int id, CancellationToken cancellationToken);
}
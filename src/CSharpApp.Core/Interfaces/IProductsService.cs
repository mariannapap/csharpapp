using CSharpApp.Core.Dtos.Requests;

namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    public Task<IReadOnlyCollection<Product?>> GetProducts(CancellationToken cancellationToken);

	public Task<Product?> GetProductById(int id, CancellationToken cancellationToken);

	public Task<Product?> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken);
}
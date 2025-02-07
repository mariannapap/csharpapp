using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Products.Queries.Handlers;

public class GetAllProductsQueryHandler(
	IProductsService productsService
) : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product?>>
{
	private readonly IProductsService _productsService = productsService;

	public async Task<IReadOnlyCollection<Product?>> Handle(
		GetAllProductsQuery request,
		CancellationToken cancellationToken
	) => await _productsService.GetProducts(cancellationToken);
}

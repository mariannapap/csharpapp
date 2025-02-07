using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Products.Queries.Handlers;

public class GetProductByIdQueryHandler(
	IProductsService productsService
) : IRequestHandler<GetProductByIdQuery, Product>
{
	private readonly IProductsService _productsService = productsService;

	public async Task<Product> Handle(
		GetProductByIdQuery request,
		CancellationToken cancellationToken
	) => await _productsService.GetProductById(request.Id, cancellationToken)
		?? throw new NotFoundException($"Product not found. ProductId: {request.Id}");
}

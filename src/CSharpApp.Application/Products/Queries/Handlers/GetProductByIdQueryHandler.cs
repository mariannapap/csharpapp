using CSharpApp.Application.Products.Queries;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Application.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
	private readonly IProductsService _productsService;
	private readonly ILogger<GetProductByIdQueryHandler> _logger;

	public GetProductByIdQueryHandler(
		IProductsService productsService,
		ILogger<GetProductByIdQueryHandler> logger
	)
	{
		_productsService = productsService;
		_logger = logger;
	}

	public async Task<Product> Handle(
		GetProductByIdQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			var product = await _productsService.GetProductById(request.Id, cancellationToken);

			if(product is null)
				throw new NotFoundException($"Product with ID {request.Id} was not found.");

			return product;
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "An error occurred while getting the product by ID.");
			throw;
		}
	}
}

using CSharpApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CSharpApp.Application.Products.Queries.Handlers;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product?>>
{
	private readonly IProductsService _productsService;
	private readonly ILogger<GetAllProductsQueryHandler> _logger;

	public GetAllProductsQueryHandler(
		IProductsService productsService,
		ILogger<GetAllProductsQueryHandler> logger
	)
	{
		_productsService = productsService;
		_logger = logger;
	}

	public async Task<IReadOnlyCollection<Product?>> Handle(
		GetAllProductsQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			return await _productsService.GetProducts(cancellationToken);
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, "An error occurred while getting products.");
			throw;
		}
	}
}

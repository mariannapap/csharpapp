using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Products.Commands.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
{
	private readonly IProductsService _productsService;

	public UpdateProductCommandHandler(IProductsService productsService)
	{
		_productsService = productsService;
	}

	public async Task<int> Handle(
		UpdateProductCommand request,
		CancellationToken cancellationToken
	) => (await _productsService.UpdateProduct(
				request.Id,
				new()
				{
					Title = request.Title,
					Price = request.Price,
					Description = request.Description
				},
				cancellationToken
			)
		)?.Id
		?? throw new ServerErrorException("Product update failed.");

}

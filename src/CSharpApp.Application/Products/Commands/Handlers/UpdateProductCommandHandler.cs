using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Products.Commands.Handlers;

public class UpdateProductCommandHandler(IProductsService productsService) : IRequestHandler<UpdateProductCommand, int>
{
	public async Task<int> Handle(
		UpdateProductCommand request,
		CancellationToken cancellationToken
	) => (await productsService.UpdateProduct(
				request.Id,
				new()
				{
					Title = request.Title,
					Price = request.Price,
					Images = request.Images
				},
				cancellationToken
			)
		)?.Id
		?? throw new ServerErrorException("Product update failed.");

}

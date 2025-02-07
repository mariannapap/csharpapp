using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Products.Commands.Handlers
{
	public class CreateProductCommandHandler(
		IProductsService productsService
	) : IRequestHandler<CreateProductCommand, int>
	{
		private readonly IProductsService _productsService = productsService;

		public async Task<int> Handle(
			CreateProductCommand command,
			CancellationToken cancellationToken
		) => (await _productsService
				.CreateProduct(
					new()
					{
						Title = command.Title,
						Description = command.Description,
						CategoryId = command.CategoryId,
						Price = command.Price,
						Images = command.Images
					}
					, cancellationToken
				)
			)
			?.Id
			?? throw new ServerErrorException("Product creation failed.");
	}
}

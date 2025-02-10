using Moq;
using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Dtos.Requests;
using CSharpApp.Core.Exceptions;
using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Commands.Handlers;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Application.Tests.Products;

public class UpdateProductCommandHandlerTests
{
	private readonly Mock<IProductsService> _productsServiceMock;
	private readonly UpdateProductCommandHandler _handler;

	public UpdateProductCommandHandlerTests()
	{
		_productsServiceMock = new Mock<IProductsService>();
		_handler = new UpdateProductCommandHandler(_productsServiceMock.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnProductId_WhenUpdateIsSuccessful()
	{
		// Arrange
		var command = new UpdateProductCommand()
		{
			Id = 1,
			Title = "Updated Product",
			Price = 100.0m,
			Description = "Updated Description"
		};
		var updatedProduct = new Product
		{
			Id = 1,
			Title = "Updated Product",
			Price = 100.0m,
			Description = "Updated Description"
		};

		_productsServiceMock
			.Setup(service => service.UpdateProduct(
				It.IsAny<int>(),
				It.IsAny<UpdateProductRequest>(),
				It.IsAny<CancellationToken>()))
			.ReturnsAsync(updatedProduct);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Equal(updatedProduct.Id, result);
	}

	[Fact]
	public async Task Handle_ShouldThrowServerErrorException_WhenUpdateFails()
	{
		// Arrange
		var command = new UpdateProductCommand()
		{
			Id = 1,
			Title = "Updated Product",
			Price = 100.0m,
			Description = "Updated Description"
		};

		_productsServiceMock
			.Setup(service => service.UpdateProduct(
				It.IsAny<int>(),
				It.IsAny<UpdateProductRequest>(),
				It.IsAny<CancellationToken>())
			)
			.ReturnsAsync((Product?)default);

		// Act & Assert
		await Assert.ThrowsAsync<ServerErrorException>(() =>
			_handler.Handle(command, CancellationToken.None));
	}
}

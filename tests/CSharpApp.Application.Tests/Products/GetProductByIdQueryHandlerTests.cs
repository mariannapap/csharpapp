using CSharpApp.Application.Products.Queries;
using CSharpApp.Application.Products.Queries.Handlers;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;
using Moq;

namespace CSharpApp.Application.Tests.Products;

public class GetProductByIdQueryHandlerTests
{
	private readonly Mock<IProductsService> _mockProductsService;
	private readonly GetProductByIdQueryHandler _handler;

	public GetProductByIdQueryHandlerTests()
	{
		_mockProductsService = new Mock<IProductsService>();
		_handler = new GetProductByIdQueryHandler(_mockProductsService.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnProduct_WhenProductExists()
	{
		// Arrange
		var product = new Product { Id = 1, Title = "Product 1" };
		_mockProductsService
			.Setup(service => service.GetProductById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act
		var result = await _handler.Handle(new GetProductByIdQuery { Id = 1 }, CancellationToken.None);

		// Assert
		Assert.Equal(product, result);
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
	{
		// Arrange
		_mockProductsService
			.Setup(service => service.GetProductById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Product?)default);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(new GetProductByIdQuery { Id = 1 }, CancellationToken.None));
		Assert.Equal("Product not found. ProductId: 1", exception.Message);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_WhenServiceThrowsException()
	{
		// Arrange
		var exception = new Exception("Test exception");
		_mockProductsService
			.Setup(service => service.GetProductById(It.IsAny<int>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(exception);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetProductByIdQuery { Id = 1 }, CancellationToken.None));
		Assert.Equal(exception.Message, ex.Message);
	}
}

using CSharpApp.Application.Products.Queries;
using CSharpApp.Application.Products.Queries.Handlers;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Moq;

namespace CSharpApp.Application.Tests.Products;

public class GetAllProductsQueryHandlerTests
{
	private readonly Mock<IProductsService> _mockProductsService;
	private readonly GetAllProductsQueryHandler _handler;

	public GetAllProductsQueryHandlerTests()
	{
		_mockProductsService = new Mock<IProductsService>();
		_handler = new GetAllProductsQueryHandler(_mockProductsService.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnProducts_WhenProductsExist()
	{
		// Arrange
		var products = new List<Product?> { new() { Id = 1, Title = "Product 1" }, new() { Id = 2, Title = "Product 2" } };
		_mockProductsService.Setup(service => service.GetProducts(It.IsAny<CancellationToken>())).ReturnsAsync(products);

		// Act
		var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

		// Assert
		Assert.Equal(products, result);
	}

	[Fact]
	public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
	{
		// Arrange
		var products = new List<Product?>();
		_mockProductsService.Setup(service => service.GetProducts(It.IsAny<CancellationToken>())).ReturnsAsync(products);

		// Act
		var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_WhenServiceThrowsException()
	{
		// Arrange
		var exception = new Exception("Test exception");
		_mockProductsService.Setup(service => service.GetProducts(It.IsAny<CancellationToken>())).ThrowsAsync(exception);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetAllProductsQuery(), CancellationToken.None));
		Assert.Equal(exception.Message, ex.Message);
	}
}

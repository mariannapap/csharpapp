using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Commands.Handlers;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Dtos.Requests;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Interfaces;
using Moq;

namespace CSharpApp.Application.Tests.Products;

public class CreateProductCommandHandlerTests
{
	private readonly Mock<IProductsService> _mockProductsService;
	private readonly CreateProductCommandHandler _handler;

	public CreateProductCommandHandlerTests()
	{
		_mockProductsService = new Mock<IProductsService>();
		_handler = new CreateProductCommandHandler(_mockProductsService.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnProductId_WhenProductIsCreated()
	{
		// Arrange
		var product = new Product { Id = 1, Title = "Product 1" };
		var command = new CreateProductCommand
		{
			Title = "Product 1",
			Description = "Description 1",
			CategoryId = 1,
			Price = 100,
			Images = new List<string> { "image1.jpg", "image2.jpg" }
		};
		_mockProductsService
			.Setup(service => service.CreateProduct(It.IsAny<CreateProductRequest>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(product);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Equal(product.Id, result);
	}

	[Fact]
	public async Task Handle_ShouldThrowServerErrorException_WhenProductCreationFails()
	{
		// Arrange
		var command = new CreateProductCommand
		{
			Title = "Product 1",
			Description = "Description 1",
			CategoryId = 1,
			Price = 100,
			Images = new List<string> { "image1.jpg", "image2.jpg" }
		};
		_mockProductsService
			.Setup(service => service.CreateProduct(It.IsAny<CreateProductRequest>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Product?)null);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<ServerErrorException>(() => _handler.Handle(command, CancellationToken.None));
		Assert.Equal("Product creation failed.", exception.Message);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_WhenServiceThrowsException()
	{
		// Arrange
		var exception = new Exception("Test exception");
		var command = new CreateProductCommand
		{
			Title = "Product 1",
			Description = "Description 1",
			CategoryId = 1,
			Price = 100,
			Images = new List<string> { "image1.jpg", "image2.jpg" }
		};
		_mockProductsService
			.Setup(service => service.CreateProduct(It.IsAny<CreateProductRequest>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(exception);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
		Assert.Equal(exception.Message, ex.Message);
	}
}

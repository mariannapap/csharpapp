using CSharpApp.Application.Categories.Queries;
using CSharpApp.Application.Categories.Queries.Handlers;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Interfaces;
using Moq;

namespace CSharpApp.Application.Tests.Categories;

public class GetAllCategoriesQueryHandlerTests
{
	private readonly Mock<ICategoryService> _mockCategoryService;
	private readonly GetAllCategoriesQueryHandler _handler;

	public GetAllCategoriesQueryHandlerTests()
	{
		_mockCategoryService = new Mock<ICategoryService>();
		_handler = new GetAllCategoriesQueryHandler(_mockCategoryService.Object);
	}

	[Fact]
	public async Task Handle_ShouldReturnCategories_WhenCategoriesExist()
	{
		// Arrange
		var categories = new List<Category?>
		{
			new Category { Id = 1, Name = "Category 1" },
			new Category { Id = 2, Name = "Category 2" }
		};
		_mockCategoryService
			.Setup(service => service.GetAllCategories(It.IsAny<CancellationToken>()))
			.ReturnsAsync(categories);

		// Act
		var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

		// Assert
		Assert.Equal(categories, result);
	}

	[Fact]
	public async Task Handle_ShouldReturnEmptyList_WhenNoCategoriesExist()
	{
		// Arrange
		var categories = new List<Category?>();
		_mockCategoryService
			.Setup(service => service.GetAllCategories(It.IsAny<CancellationToken>()))
			.ReturnsAsync(categories);

		// Act
		var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task Handle_ShouldThrowException_WhenServiceThrowsException()
	{
		// Arrange
		var exception = new Exception("Test exception");
		_mockCategoryService
			.Setup(service => service.GetAllCategories(It.IsAny<CancellationToken>()))
			.ThrowsAsync(exception);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None));
		Assert.Equal(exception.Message, ex.Message);
	}
}

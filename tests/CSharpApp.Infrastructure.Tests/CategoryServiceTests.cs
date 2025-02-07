using System.Net;
using System.Text.Json;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Exceptions;
using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Data;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace CSharpApp.Infrastructure.Tests;

public class CategoryServiceTests
{
	private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
	private readonly Mock<IOptionsSnapshot<RestApiSettings>> _mockRestApiSettings;
	private readonly RestApiSettings _restApiSettings;
	private readonly CategoryService _service;

	public CategoryServiceTests()
	{
		_mockHttpClientFactory = new Mock<IHttpClientFactory>();
		_mockRestApiSettings = new Mock<IOptionsSnapshot<RestApiSettings>>();
		_restApiSettings = new RestApiSettings
		{
			BaseUrl = "https://example.com",
			Categories = "categories"
		};
		_mockRestApiSettings.Setup(x => x.Value).Returns(_restApiSettings);
		_service = new CategoryService(_mockHttpClientFactory.Object, _mockRestApiSettings.Object);
	}

	[Fact]
	public async Task GetAllCategories_ShouldReturnCategories_WhenCategoriesExist()
	{
		// Arrange
		var categories = new List<Category?>
			{
				new Category { Id = 1, Name = "Category 1" }
			};
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(JsonSerializer.Serialize(categories))
			});
		var client = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri(_restApiSettings.BaseUrl!) };
		_mockHttpClientFactory.Setup(_ => _.CreateClient(_restApiSettings.Categories!)).Returns(client);

		// Act
		var result = await _service.GetAllCategories(CancellationToken.None);

		// Assert
		var category = Assert.Single(result);
		Assert.Equal(1, category.Id);
		Assert.Equal("Category 1", category.Name);
	}

	[Fact]
	public async Task GetAllCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
	{
		// Arrange
		var categories = new List<Category?>();
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(JsonSerializer.Serialize(categories))
			});
		var client = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri(_restApiSettings.BaseUrl!) };
		_mockHttpClientFactory
			.Setup(_ => _.CreateClient(_restApiSettings.Categories!))
			.Returns(client);

		// Act
		var result = await _service.GetAllCategories(CancellationToken.None);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public async Task GetAllCategories_ShouldThrowNotFoundException_WhenServiceReturnsNotFound()
	{
		// Arrange
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new StringContent("Not Found")
			});
		var client = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri(_restApiSettings.BaseUrl!) };
		_mockHttpClientFactory
			.Setup(_ => _.CreateClient(_restApiSettings.Categories!))
			.Returns(client);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAllCategories(CancellationToken.None));
		Assert.Equal("Not Found", exception.Message);
	}

	[Fact]
	public async Task GetAllCategories_ShouldThrowException_WhenServiceThrowsException()
	{
		// Arrange
		var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
		mockHttpMessageHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ThrowsAsync(new HttpRequestException("Test exception"));
		var client = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri(_restApiSettings.BaseUrl!) };
		_mockHttpClientFactory
			.Setup(_ => _.CreateClient(_restApiSettings.Categories!))
			.Returns(client);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetAllCategories(CancellationToken.None));
		Assert.Equal("Test exception", exception.Message);
	}
}

using System.Net;
using CSharpApp.Core.Exceptions;
using CSharpApp.Infrastructure.Extensions;

namespace CSharpApp.Infrastructure.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
	[Fact]
	public async Task HandleResponse_ShouldReturnContent_WhenStatusCodeIsSuccess()
	{
		// Arrange
		var expectedContent = "Success content";
		var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(expectedContent)
		};
		var cancellationToken = CancellationToken.None;

		// Act
		var result = await responseMessage.HandleResponse(cancellationToken);

		// Assert
		Assert.Equal(expectedContent, result);
	}

	[Fact]
	public async Task HandleResponse_ShouldThrowNotFoundException_WhenStatusCodeIsNotFound()
	{
		// Arrange
		var expectedContent = "Not Found";
		var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
		{
			Content = new StringContent(expectedContent)
		};
		var cancellationToken = CancellationToken.None;

		// Act & Assert
		var exception = await Assert.ThrowsAsync<NotFoundException>(() => responseMessage.HandleResponse(cancellationToken));
		Assert.Equal(expectedContent, exception.Message);
	}

	[Fact]
	public async Task HandleResponse_ShouldThrowBadRequestException_WhenStatusCodeIsBadRequest()
	{
		// Arrange
		var expectedContent = "Bad Request";
		var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
		{
			Content = new StringContent(expectedContent)
		};
		var cancellationToken = CancellationToken.None;

		// Act & Assert
		var exception = await Assert.ThrowsAsync<BadRequestException>(() => responseMessage.HandleResponse(cancellationToken));
		Assert.Equal(expectedContent, exception.Message);
	}

	[Fact]
	public async Task HandleResponse_ShouldThrowServerErrorException_WhenStatusCodeIsInternalServerError()
	{
		// Arrange
		var expectedContent = "Internal Server Error";
		var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
		{
			Content = new StringContent(expectedContent)
		};
		var cancellationToken = CancellationToken.None;

		// Act & Assert
		var exception = await Assert.ThrowsAsync<ServerErrorException>(() => responseMessage.HandleResponse(cancellationToken));
		Assert.Equal(expectedContent, exception.Message);
	}

	[Fact]
	public async Task HandleResponse_ShouldEnsureSuccessStatusCode_WhenStatusCodeIsOther()
	{
		// Arrange
		var expectedContent = "Other Status";
		var responseMessage = new HttpResponseMessage((HttpStatusCode)418) // I'm a teapot (arbitrary status code)
		{
			Content = new StringContent(expectedContent)
		};
		var cancellationToken = CancellationToken.None;

		// Act & Assert
		var exception = await Assert.ThrowsAsync<HttpRequestException>(() => responseMessage.HandleResponse(cancellationToken));
	}
}

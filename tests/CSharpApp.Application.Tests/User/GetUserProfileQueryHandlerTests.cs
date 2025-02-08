using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using CSharpApp.Application.User.Queries;
using CSharpApp.Application.User.Queries.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CSharpApp.Core.Dtos;

namespace CSharpApp.Application.Tests.User;

public class GetUserProfileQueryHandlerTests
{
	private readonly Mock<IAuthService> _authServiceMock;
	private readonly Mock<ITokenCacheService> _tokenCacheServiceMock;
	private readonly Mock<IOptionsSnapshot<RestApiSettings>> _optionsMock;
	private readonly Mock<ILogger<GetUserProfileQueryHandler>> _loggerMock;

	private readonly GetUserProfileQueryHandler _handler;
	private readonly RestApiSettings _settings;

	public GetUserProfileQueryHandlerTests()
	{
		_authServiceMock = new Mock<IAuthService>();
		_tokenCacheServiceMock = new Mock<ITokenCacheService>();
		_optionsMock = new Mock<IOptionsSnapshot<RestApiSettings>>();
		_loggerMock = new Mock<ILogger<GetUserProfileQueryHandler>>();

		_settings = new RestApiSettings { Username = "testuser" };
		_optionsMock.Setup(o => o.Value).Returns(_settings);

		_handler = new GetUserProfileQueryHandler(
			_authServiceMock.Object,
			_tokenCacheServiceMock.Object,
			_optionsMock.Object,
			_loggerMock.Object
		);
	}

	[Fact]
	public async Task Handle_ShouldThrowArgumentNullException_WhenUsernameIsNullOrWhiteSpace()
	{
		// Arrange
		_settings.Username = null;
		var request = new GetUserProfileQuery();
		var cancellationToken = CancellationToken.None;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(request, cancellationToken));
	}

	[Fact]
	public async Task Handle_ShouldGetNewToken_WhenTokenIsNull()
	{
		// Arrange
		var request = new GetUserProfileQuery();
		var cancellationToken = CancellationToken.None;
		var token = new Token { AccessToken = "new_token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
		_tokenCacheServiceMock.Setup(x => x.Get("testuser")).Returns((Token?)default);
		_authServiceMock.Setup(x => x.LoginAsync(cancellationToken)).ReturnsAsync(token);

		// Act
		var result = await _handler.Handle(request, cancellationToken);

		// Assert
		_tokenCacheServiceMock.Verify(x => x.Get("testuser"), Times.Once);
		_authServiceMock.Verify(x => x.LoginAsync(cancellationToken), Times.Once);
		_tokenCacheServiceMock.Verify(x => x.Set("testuser", token), Times.Once);
		_authServiceMock.Verify(x => x.GetProfileAsync("new_token", cancellationToken), Times.Once);
	}

	[Fact]
	public async Task Handle_ShouldUseExistingToken_WhenTokenIsNotNull()
	{
		// Arrange
		var request = new GetUserProfileQuery();
		var cancellationToken = CancellationToken.None;
		var token = new Token { AccessToken = "existing_token", ExpiresAt = DateTime.UtcNow.AddHours(1) };
		_tokenCacheServiceMock.Setup(x => x.Get("testuser")).Returns(token);

		// Act
		var result = await _handler.Handle(request, cancellationToken);

		// Assert
		_tokenCacheServiceMock.Verify(x => x.Get("testuser"), Times.Once);
		_authServiceMock.Verify(x => x.LoginAsync(cancellationToken), Times.Never);
		_tokenCacheServiceMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<Token>()), Times.Never);
		_authServiceMock.Verify(x => x.GetProfileAsync("existing_token", cancellationToken), Times.Once);
	}
}

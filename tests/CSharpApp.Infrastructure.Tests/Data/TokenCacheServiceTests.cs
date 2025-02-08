using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpApp.Core.Dtos;
using CSharpApp.Core.Settings;
using CSharpApp.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace CSharpApp.Infrastructure.Tests.Data;

public class TokenCacheServiceTests
{
	private readonly MemoryCache _memoryCache;
	private readonly Mock<IOptions<TokenCacheConfig>> _optionsMock;
	private readonly TokenCacheService _tokenCacheService;
	private readonly TokenCacheConfig _config;

	public TokenCacheServiceTests()
	{
		_memoryCache = new MemoryCache(new MemoryCacheOptions());
		_optionsMock = new Mock<IOptions<TokenCacheConfig>>();

		_config = new TokenCacheConfig { DurationInMin = 60 };
		_optionsMock.Setup(o => o.Value).Returns(_config);

		_tokenCacheService = new TokenCacheService(_memoryCache, _optionsMock.Object);
	}

	[Fact]
	public void Get_ShouldReturnToken_WhenTokenExistsInCache()
	{
		// Arrange
		var username = "testuser";
		var token = new Token { AccessToken = "testtoken" };
		_memoryCache.Set(username, token);

		// Act
		var result = _tokenCacheService.Get(username);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(token.AccessToken, result?.AccessToken);
	}

	[Fact]
	public void Get_ShouldReturnNull_WhenTokenDoesNotExistInCache()
	{
		// Arrange
		var username = "testuser";

		// Act
		var result = _tokenCacheService.Get(username);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void Set_ShouldCacheToken_WithCorrectExpiration()
	{
		// Arrange
		var username = "testuser";
		var token = new Token { AccessToken = GenerateJwtToken(username) };

		var jwtTokenHandler = new JwtSecurityTokenHandler();
		var jwtToken = jwtTokenHandler.ReadJwtToken(token.AccessToken);

		var cacheDuration = jwtToken.ValidTo - jwtToken.IssuedAt;
		var expectedExpiration = cacheDuration <= TimeSpan.Zero || cacheDuration > TimeSpan.FromMinutes(_config.DurationInMin)
			? TimeSpan.FromMinutes(_config.DurationInMin)
			: cacheDuration;

		// Act
		_tokenCacheService.Set(username, token);

		// Assert
		var cacheEntry = _memoryCache.Get<Token>(username);
		Assert.NotNull(cacheEntry);
		Assert.Equal(token.AccessToken, cacheEntry?.AccessToken);
	}

	private string GenerateJwtToken(string username)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes("your-256-bit-secret-your-256-bit-secret");
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity([new Claim("sub", username)]),
			Expires = DateTime.UtcNow.AddMinutes(30),
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}

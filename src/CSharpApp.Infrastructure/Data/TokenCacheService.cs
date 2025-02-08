using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;

namespace CSharpApp.Infrastructure.Data;

public class TokenCacheService : ITokenCacheService
{
	private readonly IMemoryCache _memoryCache;
	private readonly TimeSpan _cacheDuration;

	public TokenCacheService(
		IMemoryCache memoryCache,
		IOptions<TokenCacheConfig> optionsMonitor
	) => (
		_memoryCache,
		_cacheDuration
	) = (
		memoryCache,
		TimeSpan.FromMinutes(optionsMonitor.Value.DurationInMin)
	);

	public Token? Get(string username)
	{
		if(_memoryCache.TryGetValue(username, out Token? token))
		{
			return token;
		}

		return default;
	}

	public void Set(string username, Token token)
	{

		var handler = new JwtSecurityTokenHandler();
		var jwtToken = handler.ReadJwtToken(token.AccessToken);

		token.ExpiresAt = jwtToken.ValidTo;
		var cacheDuration = jwtToken.ValidTo - jwtToken.IssuedAt;

		var cacheEntryOptions = new MemoryCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = cacheDuration <= TimeSpan.Zero || cacheDuration > _cacheDuration ? _cacheDuration : cacheDuration
		};

		_memoryCache.Set(username, token, cacheEntryOptions);
	}
}

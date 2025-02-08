using CSharpApp.Core.Interfaces;
using CSharpApp.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CSharpApp.Application.User.Queries.Handlers;

public class GetUserProfileQueryHandler(
	IAuthService authService,
	ITokenCacheService tokenCacheService,
	IOptionsSnapshot<RestApiSettings> options,
	ILogger<GetUserProfileQueryHandler> logger
) : IRequestHandler<GetUserProfileQuery, UserProfile?>
{
	private readonly RestApiSettings options = options.Value;

	public async Task<UserProfile?> Handle(
		GetUserProfileQuery request,
		CancellationToken cancellationToken
	)
	{
		var username = options?.Username;
		if(string.IsNullOrWhiteSpace(username))
			throw new ArgumentNullException(nameof(username));

		var token = tokenCacheService.Get(username);
		if(token is null)
		{
			logger.LogInformation("Getting new token for {Username}", username);
			token = await authService.LoginAsync(cancellationToken);
			tokenCacheService.Set(username, token);
		}
		else
		{
			logger.LogInformation("Using existing token for {Username}", username);
		}

		return await authService.GetProfileAsync(token.AccessToken!, cancellationToken);
	}
}

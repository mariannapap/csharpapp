using System.Net.Http.Headers;
using System.Text;
using CSharpApp.Infrastructure.Extensions;

namespace CSharpApp.Infrastructure.Data;

public class AuthService(
	IHttpClientFactory httpClientFactory,
	IOptionsSnapshot<RestApiSettings> authSettings
) : IAuthService
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly RestApiSettings _authSettings = authSettings.Value;

	public async Task<Token> LoginAsync(CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_authSettings.Auth!);
		var loginData = new
		{
			email = _authSettings.Username,
			password = _authSettings.Password
		};
		var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
		var response = await client.PostAsync(_authSettings.Auth + "/login", content, cancellationToken);

		var responseBody = await response.HandleResponse(cancellationToken);
		var token = JsonSerializer.Deserialize<Token?>(responseBody);
		if(string.IsNullOrWhiteSpace(token?.AccessToken) || string.IsNullOrWhiteSpace(token.RefreshToken))
			throw new ArgumentNullException(nameof(token));
		return token;
	}

	public async Task<UserProfile?> GetProfileAsync(string accessToken, CancellationToken cancellationToken)
	{
		var client = _httpClientFactory.CreateClient(_authSettings.Auth!);
		var request = new HttpRequestMessage(HttpMethod.Get, _authSettings.Auth + "/profile");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		var response = await client.SendAsync(request, cancellationToken);

		var responseBody = await response.HandleResponse(cancellationToken);
		return JsonSerializer.Deserialize<UserProfile?>(responseBody);
	}
}

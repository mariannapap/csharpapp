namespace CSharpApp.Core.Interfaces;

public interface IAuthService
{
	public Task<Token> LoginAsync(CancellationToken cancellationToken);
	public Task<UserProfile?> GetProfileAsync(string accessToken, CancellationToken cancellationToken);
}

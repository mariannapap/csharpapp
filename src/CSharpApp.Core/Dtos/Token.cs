namespace CSharpApp.Core.Dtos;

public class Token
{
	[JsonPropertyName("access_token")]
	public string? AccessToken { get; set; }

	[JsonPropertyName("refresh_token")]
	public string? RefreshToken { get; set; }

	public DateTime ExpiresAt { get; set; }
}
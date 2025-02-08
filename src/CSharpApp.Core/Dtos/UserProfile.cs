namespace CSharpApp.Core.Dtos;

public class UserProfile
{
	[JsonPropertyName("id")]
	public int? Id { get; set; }

	[JsonPropertyName("email")]
	public string? Email { get; set; }

	[JsonPropertyName("password")]
	public string? Password { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("role")]
	public string? Role { get; set; }

	[JsonPropertyName("avatar")]
	public string? Avatar { get; set; }

	[JsonPropertyName("creationAt")]
	public DateTime? CreationAt { get; set; }

	[JsonPropertyName("updatedAt")]
	public DateTime? UpdatedAt { get; set; }
}

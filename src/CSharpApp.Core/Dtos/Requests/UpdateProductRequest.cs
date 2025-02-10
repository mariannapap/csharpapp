namespace CSharpApp.Core.Dtos.Requests;

public class UpdateProductRequest
{
	public string? Name{ get; init; }
	public decimal? Price{ get; init; }
	public string? Description{ get; init; }
}

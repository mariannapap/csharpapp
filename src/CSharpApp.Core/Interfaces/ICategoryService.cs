namespace CSharpApp.Core.Interfaces;

public interface ICategoryService
{
	public Task<IReadOnlyCollection<Category?>> GetAllCategories(CancellationToken cancellationToken);
}

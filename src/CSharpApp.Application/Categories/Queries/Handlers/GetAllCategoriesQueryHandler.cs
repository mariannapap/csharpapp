using CSharpApp.Core.Interfaces;

namespace CSharpApp.Application.Categories.Queries.Handlers;

public class GetAllCategoriesQueryHandler(
	ICategoryService categoryService
) : IRequestHandler<GetAllCategoriesQuery, IReadOnlyCollection<Category?>>
{
	private readonly ICategoryService _categoryService = categoryService;

	public async Task<IReadOnlyCollection<Category?>> Handle(
		GetAllCategoriesQuery request,
		CancellationToken cancellationToken
	) => await _categoryService.GetAllCategories(cancellationToken);
}

using Asp.Versioning;
using CSharpApp.Api.Middleware;
using CSharpApp.Application.Categories.Queries;
using CSharpApp.Application.Products.Commands;
using CSharpApp.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning(options =>
{
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "My API",
		Version = "v1"
	});
	options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
	options.TagActionsBy(api => new List<string> { api.GroupName });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
	});
}

//app.UseHttpsRedirection();

app.UseMiddleware<PerformanceLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder
	.MapGet(
		"/api/v{version:apiVersion}/getproducts",
		async (IMediator mediator, CancellationToken cancellationToken) =>
			await mediator.Send(new GetAllProductsQuery(), cancellationToken)
	)
	.WithName("GetProducts")
	.HasApiVersion(1.0);

versionedEndpointRouteBuilder
	.MapGet(
		"/api/v{version:apiVersion}/products/{id}",
		async (int id, IMediator mediator, CancellationToken cancellationToken) =>
			await mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken)
	)
	.WithName("GetProductById")
	.HasApiVersion(1.0);

versionedEndpointRouteBuilder
	.MapPost(
		"/api/v{version:apiVersion}/products",
		async ([FromBody] CreateProductCommand command, IMediator mediator, CancellationToken cancellationToken) =>
			await mediator.Send(command, cancellationToken)
	)
	.WithName("CreateProduct")
	.HasApiVersion(1.0);

versionedEndpointRouteBuilder
	.MapGet(
		"/api/v{version:apiVersion}/categories",
		async (IMediator mediator, CancellationToken cancellationToken) =>
			await mediator.Send(new GetAllCategoriesQuery(), cancellationToken)
	)
	.WithName("GetAllCategories")
	.HasApiVersion(1.0);

app.Run();

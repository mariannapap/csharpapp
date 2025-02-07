using CSharpApp.Api.Middleware;
using CSharpApp.Application.Products.Queries;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Logging.ClearProviders().AddSerilog(logger);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDefaultConfiguration();
builder.Services.AddHttpConfiguration();
builder.Services.AddProblemDetails();
builder.Services.AddApiVersioning();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

var versionedEndpointRouteBuilder = app.NewVersionedApi();

versionedEndpointRouteBuilder.MapGet("/api/v{version:apiVersion}/getproducts", async (IMediator mediator, CancellationToken cancellationToken) =>
{
	var products = await mediator.Send(new GetAllProductsQuery(), cancellationToken);
	return products;
})
.WithName("GetProducts")
.HasApiVersion(1.0);

versionedEndpointRouteBuilder.MapGet("/api/v{version:apiVersion}/products/{id}", async (int id, IMediator mediator, CancellationToken cancellationToken) =>
{
	var product = await mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
	return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.HasApiVersion(1.0);

app.Run();
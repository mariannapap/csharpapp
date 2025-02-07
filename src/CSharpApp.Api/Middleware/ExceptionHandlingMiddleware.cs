using CSharpApp.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext httpContext)
	{
		try
		{
			await _next(httpContext);
		}
		catch(NotFoundException ex)
		{
			_logger.LogError(ex, ex.Message);
			await HandleExceptionAsync(httpContext, StatusCodes.Status404NotFound, ex.Message, "Not Found");
		}
		catch(BadRequestException ex)
		{
			_logger.LogError(ex, ex.Message);
			await HandleExceptionAsync(httpContext, StatusCodes.Status400BadRequest, ex.Message, "Bad Request");
		}
		catch(ServerErrorException ex)
		{
			_logger.LogError(ex, ex.Message);
			await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, ex.Message, "Internal Server Error");
		}
		catch(Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, "An unexpected fault happened. Try again later.", "Internal Server Error");
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, int statusCode, string detail, string title)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = statusCode;

		var problemDetails = new ProblemDetails
		{
			Status = statusCode,
			Title = title,
			Detail = detail,
			Instance = context.Request.Path
		};

		return context.Response.WriteAsJsonAsync(problemDetails);
	}
}

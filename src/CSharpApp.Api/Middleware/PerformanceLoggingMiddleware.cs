using System.Diagnostics;

namespace CSharpApp.Api.Middleware;

public class PerformanceLoggingMiddleware(
	RequestDelegate next,
	ILogger<PerformanceLoggingMiddleware> logger
)
{
	private readonly RequestDelegate _next = next;
	private readonly ILogger<PerformanceLoggingMiddleware> _logger = logger;

	public async Task InvokeAsync(HttpContext context)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		await _next(context);

		stopwatch.Stop();
		var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

		if(elapsedMilliseconds > 1000) // Log requests taking more than 1000ms (1 second)
		{
			_logger.LogWarning(
				"Long Running Request: {Method} {Path} took {ElapsedMilliseconds} ms",
				context.Request.Method,
				context.Request.Path,
				elapsedMilliseconds
			);
		}
		else
		{
			_logger.LogInformation(
				"Request: {Method} {Path} took {ElapsedMilliseconds} ms",
				context.Request.Method,
				context.Request.Path,
				elapsedMilliseconds
			);
		}
	}
}

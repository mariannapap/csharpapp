using CSharpApp.Core.Exceptions;

namespace CSharpApp.Infrastructure.Extensions;

public static class HttpResponseMessageExtensions
{
	public static async Task<string> HandleResponse(
		this HttpResponseMessage response,
		CancellationToken cancellationToken
	)
	{
		var content = await response.Content.ReadAsStringAsync(cancellationToken);

		switch(response.StatusCode)
		{
			case System.Net.HttpStatusCode.NotFound:
				throw new NotFoundException(content);
			case System.Net.HttpStatusCode.BadRequest:
				throw new BadRequestException(content);
			case System.Net.HttpStatusCode.InternalServerError:
				throw new ServerErrorException(content);
			default:
				response.EnsureSuccessStatusCode();
				break;
		}

		return content;
	}
}

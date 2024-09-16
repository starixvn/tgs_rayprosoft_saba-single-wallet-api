using MediatR;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.Common;

namespace SabaSingleWallet.API.Services._Shared;

public class LoggingBehavior<TRequest, TResponse>(
		ISapaApiMessagePublishHandler messagePublisher
		) : IPipelineBehavior<TRequest, TResponse>
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		// Pre-processing
		Console.WriteLine($"Handling {typeof(TRequest).Name}");

		var response = await next();

		// Post-processing
		Console.WriteLine($"Handled {typeof(TResponse).Name}");

		_ = await GenerateResponseAsync(request, response, cancellationToken);

		return response;
	}

	private async Task<TResponse> GenerateResponseAsync(TRequest request, TResponse result, CancellationToken cancellationToken)
	{
		//TODO publish new message format
		var msg = new ApiLogMessage()
		{
			Request = request,
			Response = result
		};

		await messagePublisher.LogApi(msg, cancellationToken);

		return result;
	}
}
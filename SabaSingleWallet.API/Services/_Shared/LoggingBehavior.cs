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

		var response = await next();

		// Post-processing
		_ = await GenerateResponseAsync(request, response, cancellationToken);

		return response;
	}

	private async Task<TResponse> GenerateResponseAsync(TRequest request, TResponse result, CancellationToken cancellationToken)
	{
		var msg = new ApiLogMessage()
		{
			Request = request,
			Response = result
		};

		await messagePublisher.LogApi(msg, cancellationToken);

		return result;
	}
}
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.Common;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class GetBalanceHandler(
		ICashServiceClient cashClient,
		ISapaApiMessagePublishHandler messagePublisher,
		ISentryClient sentryClient
	) : IRequestHandler<GetBalanceRequest, GetBalanceResult>

{
	public async Task<GetBalanceResult> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var playerCode = request.Message.UserId.ToPlayerCode();

			var response = await cashClient.GameProviderGetBalanceAsync(playerCode, cancellationToken).ConfigureAwait(false);

			var result = new GetBalanceResult(SabaStatusCodeConst.SuccessCode, request.Message.UserId, response.Balance, DateTime.UtcNow.ToString());

			// Temp remove

			//await messagePublisher.LogApi(new ApiLogMessage()
			//{
			//	Key = request.Key,
			//	Action = request.Message.Action,
			//	Request = request,
			//	Response = result
			//}, cancellationToken);

			return result;
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
			return new GetBalanceResult(SabaStatusCodeConst.ApiInternalErrorCode, null, null, null);
		}
	}
}
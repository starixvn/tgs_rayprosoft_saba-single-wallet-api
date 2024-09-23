using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Models.Balance;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class GetBalanceHandler(
		ICashServiceClient cashClient,
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

			return result;
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
			return new GetBalanceResult(SabaStatusCodeConst.ApiInternalErrorCode, null, null, null);
		}
	}
}
using AutoMapper;
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.Common;
using SabaSingleWallet.Repositories;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class PlaceBetHandler(

		ICashServiceClient cashClient,
		ISapaApiMessagePublishHandler messagePublisher,
		IPlayerRepository playerRepository,
		IMapper mapper,
		ISentryClient sentryClient
	) : IRequestHandler<PlaceBetRequest, PlaceBetResult>
{
	public async Task<PlaceBetResult> Handle(PlaceBetRequest request, CancellationToken cancellationToken)
	{
		try
		{
			var playerCode = request.message.UserId.ToPlayerCode();

			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(
				playerCode,
				request.message.DebitAmount,
				new ProviderInfo(
					ProviderConst.ProviderCode,
					request.message.RefId,
					request.message.OperationId,
					request.message.Action
					),
				cancellationToken: cancellationToken
				).ConfigureAwait(false);

			if (!string.IsNullOrWhiteSpace(deductBalanceResult.ErrorMessage))
			{
				return await GenerateResponseAsync(request, new PlaceBetResult(deductBalanceResult.ErrorCode, request.message.RefId, null), cancellationToken);
			}

			var criteria = mapper.Map<PlaceBetCriteria>(request.message);
			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetSPResult>(criteria);

			if (spResult.StatusCode != SabaStatusCodeConst.SuccessCode)
			{
				return await GenerateResponseAsync(request, new PlaceBetResult(spResult.StatusCode, request.message.RefId, null), cancellationToken);
			}

			var res = new PlaceBetResult(SabaStatusCodeConst.SuccessCode, request.message.RefId, spResult.Licenseetxid);

			return await GenerateResponseAsync(request, res, cancellationToken);
		}
		catch (Exception ex)
		{
			_ = sentryClient.CaptureException(ex);
			return await GenerateResponseAsync(request, new PlaceBetResult(SabaStatusCodeConst.ApiInternalErrorCode, request.message.RefId, null), cancellationToken);
		}
	}

	private async Task<PlaceBetResult> GenerateResponseAsync(PlaceBetRequest request, PlaceBetResult result, CancellationToken cancellationToken)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = request.key,
			Action = request.message.Action,
			Request = request,
			Response = result
		}, cancellationToken);

		return result;
	}
}
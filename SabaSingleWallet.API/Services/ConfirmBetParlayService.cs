using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBetParlay;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetParlayService(
	ISapaApiMessagePublishHandler messagePublisher,
	IMapper mapper,
	IPlayerRepository playerRepository,
	ICashServiceClient client
) : IGetService<ConfirmBetParlayRequest, ConfirmBetParlayResult>
{
	public async ValueTask<ConfirmBetParlayResult> GetAsync(ConfirmBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.message.UserId.ToPlayerCode();

			foreach (var ticket in CreateConfirmBetCriteria(query))
			{
				var inserted = await playerRepository.CallStoredProcedureAsync<ConfirmBetCriteria, ConfirmBetSPResult>(ticket, StoreProcedureMapper.ConfirmBetSP);

				if (inserted.StatusCode < 0)
				{
					var insertError = CreateResult(506);

					return await WriteLogAndReturnResponseAsync(query, insertError, cancellationToken);
				}

				var confirmData = HandleMessageMapper.ConfirmBet(ticket, playerCode);

				await messagePublisher.HandleBetAsync(confirmData, cancellationToken);

				var increaseError = await IncreaseBalanceIfNeededAsync(playerCode, ticket, cancellationToken);

				if (increaseError != null)
				{
					return await WriteLogAndReturnResponseAsync(query, increaseError, cancellationToken);
				}
			}

			var currentBalance = await client.GameProviderGetBalanceAsync(playerCode, cancellationToken);
			var successResponse = CreateResult(0, currentBalance.Balance);

			return await WriteLogAndReturnResponseAsync(query, successResponse, cancellationToken);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return CreateResult(-1, 0);
		}
	}

	private async ValueTask<ConfirmBetParlayResult?> IncreaseBalanceIfNeededAsync(
		string playerCode,
		ConfirmBetCriteria ticket,
		CancellationToken cancellationToken)
	{
		if (ticket.IsOddsChanged && ticket.CreditAmount > 0)
		{
			var addBalanceResult = await client.GameProviderAddBalanceAsync(
					playerCode,
					ticket.CreditAmount,
					"",
					ticket.RefId,
					null,
					cancellationToken
				).ConfigureAwait(false);

			if (addBalanceResult.ErrorCode != 0)
			{
				return CreateResult(307);
			}
		}

		return null;
	}

	private List<ConfirmBetCriteria> CreateConfirmBetCriteria(ConfirmBetParlayRequest query)
	{
		var result = new List<ConfirmBetCriteria>();

		foreach (var ticket in query.message.Txns)
		{
			var criteria = mapper.Map<ConfirmBetCriteria>(ticket);

			criteria.Action = query.message.Action;
			criteria.OperationId = query.message.OperationId;
			criteria.UpdateTime = DateTime.Parse(query.message.UpdateTime);
			criteria.TransactionTime = DateTime.Parse(query.message.TransactionTime);

			result.Add(criteria);
		}

		return result;
	}

	private async ValueTask<ConfirmBetParlayResult> WriteLogAndReturnResponseAsync(ConfirmBetParlayRequest request, ConfirmBetParlayResult response, CancellationToken cancellationToken)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = request.key,
			Action = request.message.Action,
			Request = request,
			Response = response
		}, cancellationToken);

		return response;
	}

	private static ConfirmBetParlayResult CreateResult(int code, decimal balance = 0)
	{
		return code switch
		{
			0 => new ConfirmBetParlayResult("0", balance, null),
			307 => new ConfirmBetParlayResult("307", balance, "Cannot Credit player. Please Try Again Later"),
			506 => new ConfirmBetParlayResult("506", balance, "Unable To Processed. Please Try Again Later"),
			_ => new ConfirmBetParlayResult("-1", balance, "Undefined"),
		};
	}
}
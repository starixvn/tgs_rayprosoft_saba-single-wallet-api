using AutoMapper;
using MediatR;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Helpers;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.ConfirmBetParlay;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.ConfirmBetCriterias;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;

namespace SabaSingleWallet.API.Services;

public class ConfirmBetParlayService(
	ISapaApiMessagePublishHandler messagePublisher,
	IMapper mapper,
	IPlayerRepository playerRepository,
	ICashServiceClient client
) : IRequestHandler<ConfirmBetParlayRequest, ConfirmBetParlayResult>
{
	public async Task<ConfirmBetParlayResult> Handle(ConfirmBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var response = await ProccessRequestAsync(query, cancellationToken);

			await messagePublisher.LogApi(new ApiLogMessage()
			{
				Key = query.key,
				Action = query.message.Action,
				Request = query,
				Response = response
			}, cancellationToken);

			return response;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return new ConfirmBetParlayResult(0, SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	public async ValueTask<ConfirmBetParlayResult> ProccessRequestAsync(ConfirmBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var playerCode = query.message.UserId.ToPlayerCode();

			foreach (var ticket in CreateConfirmBetCriteria(query))
			{
				var inserted = await playerRepository.CallStoredProcedureAsync<ConfirmBetCriteria, ConfirmBetSPResult>(ticket, StoreProcedureMapper.ConfirmBetSP);

				if (!SabaStatusCodeConst.IsSuccess(inserted.StatusCode))
				{
					return new ConfirmBetParlayResult(0, inserted.StatusCode);
				}

				var confirmData = HandleMessageMapper.ConfirmBet(ticket, playerCode);

				await messagePublisher.HandleBetAsync(confirmData, cancellationToken);

				var increaseError = await IncreaseBalanceIfNeededAsync(playerCode, ticket, cancellationToken);

				if (increaseError != null)
				{
					return increaseError;
				}
			}

			var currentBalance = await client.GameProviderGetBalanceAsync(playerCode, cancellationToken);

			return new ConfirmBetParlayResult(currentBalance.Balance);
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return new ConfirmBetParlayResult(0, SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private async ValueTask<ConfirmBetParlayResult?> IncreaseBalanceIfNeededAsync(
		string playerCode,
		ConfirmBetCriteria ticket,
		CancellationToken cancellationToken)
	{
		if (ticket.IsOddsChanged && ticket.CreditAmount > 0)
		{
			var provider = ProviderConst.CreateProviderInfo(ticket.RefId, ticket.OperationId, ticket.Action);
			var result = await client.GameProviderAddBalanceAsync(playerCode, ticket.CreditAmount, provider, "", cancellationToken).ConfigureAwait(false);

			if (!SabaStatusCodeConst.IsSuccess(result.ErrorCode))
			{
				return new ConfirmBetParlayResult(0, result.ErrorCode);
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
}
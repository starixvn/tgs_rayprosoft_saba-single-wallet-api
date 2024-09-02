using AutoMapper;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Messages;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetParlayService(
	ICashServiceClient cashClient,
	ISapaApiMessagePublishHandler messagePublisher,
	IPlayerRepository playerRepository,
	IMapper mapper
) : IGetService<PlaceBetParlayRequest, PlaceBetParlayResult>
{
	public async ValueTask<PlaceBetParlayResult> GetAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken = default)
	{
		try
		{
			var deductError = await DeductBalanceAsync(query, cancellationToken);

			if (deductError != null)
			{
				await WriteLogAsync(query, deductError, cancellationToken);

				return deductError;
			}

			var ticketInfos = new List<TicketInfoMapping>();
			var error = false;

			foreach (var ticket in CreatePlaceBetCriteria(query))
			{
				var insertTicketResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>(
					ticket,
					StoreProcedureMapper.PlaceBetSP
				);

				ticketInfos.Add(new TicketInfoMapping(ticket.RefId, insertTicketResult.Licenseetxid));

				if (!error && insertTicketResult.SpResult != 0)
				{
					error = true;
				}
			}

			if (error)
			{
				await RefundAsync(query, cancellationToken);

				var errorResponse = new PlaceBetParlayResult("506", ticketInfos, "Unable To Processed. Please Try Again Later");

				await WriteLogAsync(query, errorResponse, cancellationToken);

				return errorResponse;
			}

			var successResponse = new PlaceBetParlayResult("0", ticketInfos, "");

			await WriteLogAsync(query, successResponse, cancellationToken);

			return successResponse;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());
			return new PlaceBetParlayResult("203", null, "");
		}
	}

	private async ValueTask<PlaceBetParlayResult?> DeductBalanceAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken)
	{
		var playerCode = query.message.UserId.ToPlayerCode();
		var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(
				playerCode,
				query.message.TotalBetAmount,
				"",
				query.message.OperationId,
				null,
				cancellationToken
			).ConfigureAwait(false);

		if (deductBalanceResult.ErrorCode != 0)
		{
			var tickets = GetTicketInfos(query.message.Txns.Select(t => t.RefId).ToList(), null);
			var result = CreateErrorResult(deductBalanceResult.ErrorCode, tickets);

			return result;
		}

		return null;
	}

	private async ValueTask RefundAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken)
	{
		var playerCode = query.message.UserId.ToPlayerCode();

		_ = await cashClient.GameProviderAddBalanceAsync(
				playerCode,
				query.message.TotalBetAmount,
				"",
				query.message.OperationId,
				null,
				cancellationToken
			).ConfigureAwait(false);
	}

	private async ValueTask WriteLogAsync(PlaceBetParlayRequest request, PlaceBetParlayResult response, CancellationToken cancellationToken)
	{
		await messagePublisher.LogApi(new ApiLogMessage()
		{
			Key = request.key,
			Action = request.message.Action,
			Request = request,
			Response = response
		}, cancellationToken);
	}

	private List<PlaceBetCriteria> CreatePlaceBetCriteria(PlaceBetParlayRequest query)
	{
		var result = new List<PlaceBetCriteria>();

		foreach (var ticket in query.message.Txns)
		{
			var criteria = mapper.Map<PlaceBetCriteria>(query.message);
			var singleBet = ticket.Detail!.FirstOrDefault(detail => detail is SingleBetDetail) as SingleBetDetail;

			MapParlayTicket(criteria, ticket);

			if (singleBet != null)
			{
				var match = query.message.TicketDetail.FirstOrDefault(match => match.MatchId == singleBet.MatchId);

				if (match != null)
				{
					MapSingleBetTicket(criteria, match);
				}
			}

			result.Add(criteria);
		}

		return result;
	}

	private static void MapParlayTicket(PlaceBetCriteria criteria, ComboInfo ticket)
	{
		criteria.RefId = ticket.RefId;
		criteria.BetTypeName = ticket.ParlayType;
		criteria.BetAmount = ticket.BetAmount;
		criteria.CreditAmount = ticket.CreditAmount;
		criteria.DebitAmount = ticket.DebitAmount;
		criteria.BetType = 29;
	}

	private static void MapSingleBetTicket(PlaceBetCriteria criteria, TicketDetail ticket)
	{
		criteria.MatchId = ticket.MatchId;
		criteria.HomeId = ticket.HomeId;
		criteria.AwayId = ticket.AwayId;
		criteria.HomeName = ticket.HomeName;
		criteria.AwayName = ticket.AwayName;
		criteria.KickOffTime = DateTime.Parse(ticket.KickOffTime);
		criteria.SportType = ticket.SportType;
		criteria.SportTypeName = ticket.SportTypeName;
		criteria.BetType = ticket.BetType;
		criteria.BetTypeName = ticket.BetTypeName;
		criteria.OddsId = ticket.OddsId;
		criteria.Odds = ticket.Odds;
		criteria.OddsType = ticket.OddsType;
		criteria.BetChoice = ticket.BetChoice;
		criteria.BetChoice_en = ticket.BetChoice_en;
		criteria.LeagueId = ticket.LeagueId;
		criteria.IsLive = ticket.IsLive;
		criteria.Point = ticket.Point ?? string.Empty;
		criteria.Point2 = ticket.Point2 ?? string.Empty;
		criteria.BetTeam = ticket.BetTeam;
		criteria.HomeScore = ticket.HomeScore;
		criteria.AwayScore = ticket.AwayScore;
		criteria.BaStatus = ticket.BaStatus;
		criteria.Excluding = ticket.Excluding ?? string.Empty;
		criteria.LeagueName_en = ticket.LeagueName_en;
		criteria.SportTypeName_en = ticket.SportTypeName_en;
		criteria.HomeName_en = ticket.HomeName_en;
		criteria.AwayName_en = ticket.AwayName_en;
		criteria.BetTypeName_en = ticket.BetTypeName_en;
		criteria.MatchDateTime = DateTime.Parse(ticket.MatchDateTime);
	}

	private static List<TicketInfoMapping> GetTicketInfos(List<string> refIds, string? licenseeTxId)
		=> refIds.Select(id => new TicketInfoMapping(licenseeTxId, id)).ToList();

	private static PlaceBetParlayResult CreateErrorResult(int code, List<TicketInfoMapping> tickets)
	{
		return code switch
		{
			1 => new PlaceBetParlayResult("1", tickets, "Duplicate Transaction"),
			2 => new PlaceBetParlayResult("307", tickets, "Invalid Amount"),
			502 => new PlaceBetParlayResult("502", tickets, "Player Has Insufficient Funds"),
			_ => new PlaceBetParlayResult("-1", tickets, "Undefined"),
		};
	}
}
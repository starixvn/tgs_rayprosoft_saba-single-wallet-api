using AutoMapper;
using Newtonsoft.Json;
using SabaSingleWallet.API.Extensions;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.Common;
using SabaSingleWallet.Core;
using SabaSingleWallet.Repositories.Helpers;
using SabaSingleWallet.Repositories.Models.PlaceBet;
using Tgs.CashService.Client;
using Tgs.GameRecord.Saba.Common;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Services;

public class PlaceBetParlayService(
	ICashServiceClient cashClient,
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
				return deductError;
			}

			var response = await InsertTicketsAsync(query, cancellationToken);

			return response;
		}
		catch (Exception ex)
		{
			Console.Write(ex.ToString());

			return new PlaceBetParlayResult(null, SabaStatusCodeConst.ApiInternalErrorCode);
		}
	}

	private async ValueTask<PlaceBetParlayResult?> DeductBalanceAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken)
	{
		var playerCode = query.message.UserId.ToPlayerCode();

		foreach (var ticket in CreatePlaceBetCriteria(query))
		{
			var provider = ProviderConst.CreateProviderInfo(ticket.RefId, ticket.OperationId, ticket.Action);
			var deductBalanceResult = await cashClient.GameProviderDeductBalanceAsync(playerCode, ticket.DebitAmount, provider, "", cancellationToken).ConfigureAwait(false);

			if (!SabaStatusCodeConst.IsSuccess(deductBalanceResult.ErrorCode))
			{
				var tickets = GetErrorTickets(query.message.Txns.Select(t => t.RefId).ToList());

				return new PlaceBetParlayResult(tickets, deductBalanceResult.ErrorCode);
			}
		}

		return null;
	}

	private async ValueTask<PlaceBetParlayResult> InsertTicketsAsync(PlaceBetParlayRequest query, CancellationToken cancellationToken)
	{
		var ticketInfos = new List<TicketInfoMapping>();

		foreach (var ticket in CreatePlaceBetCriteria(query))
		{
			var spResult = await playerRepository.CallStoredProcedureAsync<PlaceBetCriteria, PlaceBetSPResult>(ticket, StoreProcedureMapper.PlaceBetSP);

			ticketInfos.Add(new TicketInfoMapping(ticket.RefId, spResult.Licenseetxid));

			if (!SabaStatusCodeConst.IsSuccess(spResult.StatusCode))
			{
				var tickets = GetErrorTickets(query.message.Txns.Select(t => t.RefId).ToList());

				return new PlaceBetParlayResult(tickets);
			}
		}

		return new PlaceBetParlayResult(ticketInfos);
	}

	private List<PlaceBetCriteria> CreatePlaceBetCriteria(PlaceBetParlayRequest query)
	{
		var result = new List<PlaceBetCriteria>();

		foreach (var ticket in query.message.Txns)
		{
			var criteria = mapper.Map<PlaceBetCriteria>(query.message);
			var singleBet = ticket.Detail!.FirstOrDefault(detail => detail is SingleBetDetail) as SingleBetDetail;

			MapParlayTicket(criteria, ticket, query.message.TicketDetail);

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

	private static void MapParlayTicket(PlaceBetCriteria criteria, ComboInfo ticket, List<TicketDetail> ticketDetails)
	{
		criteria.RefId = ticket.RefId;
		criteria.ParlayType = ticket.ParlayType;
		criteria.BetAmount = ticket.BetAmount;
		criteria.CreditAmount = ticket.CreditAmount;
		criteria.DebitAmount = ticket.DebitAmount;
		criteria.BetType = 29;
		criteria.BetTypeName = "Parlay";
		criteria.BetTypeName_en = "Parlay";
		criteria.KickOffTime = null;
		criteria.MatchDateTime = null;
		criteria.Detail = JsonConvert.SerializeObject(new { Ticket = ticket, TicketDetail = ticketDetails });
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

	private static List<TicketInfoMapping> GetErrorTickets(List<string> refIds)
		=> refIds.Select(id => new TicketInfoMapping(null, id)).ToList();
}
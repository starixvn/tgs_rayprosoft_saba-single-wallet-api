using MediatR;

namespace SabaSingleWallet.API.Models.CancelBet;

public record CancelBetRequest(
	   string key,
	   CancelBetData message
	   ) : IRequest<CancelBetResult>;
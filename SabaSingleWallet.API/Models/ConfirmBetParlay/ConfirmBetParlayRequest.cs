using MediatR;

namespace SabaSingleWallet.API.Models.ConfirmBetParlay;

public record ConfirmBetParlayRequest(
   string key,
   ConfirmBetParlayData message
   ) : IRequest<ConfirmBetParlayResult>;
using MediatR;

namespace SabaSingleWallet.API.Models.SettleEnt;

public record SettleEntRequest(
	   string key,
	   SettleEntData message
	   ) : IRequest<SettleEntResult>;
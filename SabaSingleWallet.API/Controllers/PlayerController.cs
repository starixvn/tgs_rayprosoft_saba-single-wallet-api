using MediatR;
using Microsoft.AspNetCore.Mvc;
using SabaSingleWallet.API.Models.AdjustBalance;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.API.Models.CancelBetEnt;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.API.Models.ConfirmBet3rd;
using SabaSingleWallet.API.Models.ConfirmBetParlay;
using SabaSingleWallet.API.Models.GetTicketInfo;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.API.Models.Placebet3rd;
using SabaSingleWallet.API.Models.Placebetent;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.API.Models.SettleEnt;
using SabaSingleWallet.API.Models.Unsettle;

namespace SabaSingleWallet.API.Controllers
{
	[Route("api/player")]
	[ApiController]
	public class PlayerController(IMediator mediator) : ControllerBase
	{
		[HttpPost("placebet")]
		public Task<PlaceBetResult> PlaceBetAsync(PlaceBetRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("getbalance")]
		public Task<GetBalanceResult> GetBalanceAsync(GetBalanceRequest request)
			 => mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("settle")]
		public Task<SettleResult> SettleAsync(SettleRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("confirmbet")]
		public Task<ConfirmBetResult> ConfirmBetAsync(ConfirmBetRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("cancelbet")]
		public Task<CancelBetResult> CancelBetAsync(CancelBetRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("resettle")]
		public Task<ResettleResult> ResettleAsync(ResettleRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("unsettle")]
		public Task<UnsettleResult> UnsettleAsync(UnsettleRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("placebetparlay")]
		public Task<PlaceBetParlayResult> PlacebetParlayAsync(PlaceBetParlayRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("confirmbetparlay")]
		public Task<ConfirmBetParlayResult> ConfirmBetParlayAsync(ConfirmBetParlayRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("placebet3rd")]
		public Task<Placebet3rdResult> Placebet3rdAsync(Placebet3rdRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("confirmbet3rd")]
		public Task<ConfirmBet3rdResult> Confirmbet3rdAsync(ConfirmBet3rdRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("placebetent")]
		public Task<PlacebetentResult> PlaceBetEntAsync(PlacebetentRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("settleent")]
		public Task<SettleEntResult> SettleEntAsync(SettleEntRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("cancelbetent")]
		public Task<CancelBetEntResult> CancelBetEntAsync(CancelBetEntRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("getticketinfo")]
		public Task<GetTicketInfoResult> GetTicketInfoAsync(GetTicketInfoRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);

		[HttpPost("adjustbalance")]
		public Task<AdjustBalanceResult> AdjustBalanceAsync(AdjustBalanceRequest request)
			=> mediator.Send(request, cancellationToken: HttpContext.RequestAborted);
	}
}
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
		public async Task<PlaceBetResult> PlaceBetAsync(PlaceBetRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("getbalance")]
		public async Task<GetBalanceResult> GetBalanceAsync(GetBalanceRequest request)
			 => await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("settle")]
		public async Task<SettleResult> SettleAsync(SettleRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("confirmbet")]
		public async Task<ConfirmBetResult> ConfirmBetAsync(ConfirmBetRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("cancelbet")]
		public async Task<CancelBetResult> CancelBetAsync(CancelBetRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("resettle")]
		public async Task<ResettleResult> ResettleAsync(ResettleRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("unsettle")]
		public async Task<UnsettleResult> UnsettleAsync(UnsettleRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("placebetparlay")]
		public async Task<PlaceBetParlayResult> PlacebetParlayAsync(PlaceBetParlayRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("confirmbetparlay")]
		public async Task<ConfirmBetParlayResult> ConfirmBetParlayAsync(ConfirmBetParlayRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("placebet3rd")]
		public async Task<Placebet3rdResult> Placebet3rdAsync(Placebet3rdRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("confirmbet3rd")]
		public async Task<ConfirmBet3rdResult> Confirmbet3rdAsync(ConfirmBet3rdRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("placebetent")]
		public async Task<PlacebetentResult> PlaceBetEntAsync(PlacebetentRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("settleent")]
		public async Task<SettleEntResult> SettleEntAsync(SettleEntRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("cancelbetent")]
		public async Task<CancelBetEntResult> CancelBetEntAsync(CancelBetEntRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("getticketinfo")]
		public async Task<GetTicketInfoResult> GetTicketInfoAsync(GetTicketInfoRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

		[HttpPost("adjustbalance")]
		public async Task<AdjustBalanceResult> AdjustBalanceAsync(AdjustBalanceRequest request)
			=> await mediator.Send(request, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);
	}
}
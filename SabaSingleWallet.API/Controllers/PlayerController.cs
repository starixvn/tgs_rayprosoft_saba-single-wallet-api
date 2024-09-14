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
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Controllers
{
	[Route("api/player")]
	[ApiController]
	public class PlayerController(IMediator mediator) : ControllerBase
	{
		[HttpPost("placebet")]
		public async ValueTask<PlaceBetResult> PlaceBetAsync(PlaceBetRequest request)
		{
			var result = await mediator.Send(request);

			return result;
		}

		[HttpPost("getbalance")]
		public async ValueTask<GetBalanceResult> GetBalanceAsync(GetBalanceRequest request)
		{
			var result = await mediator.Send(request);

			return result;
		}

		[HttpPost("settle")]
		public async ValueTask<SettleResult> SettleAsync(SettleRequest request)
		{
			var result = await mediator.Send(request);

			return result;
		}

		[HttpPost("confirmbet")]
		public async ValueTask<ConfirmBetResult> ConfirmBetAsync(ConfirmBetRequest request)
		{
			var result = await mediator.Send(request);

			return result;
		}

		[HttpPost("cancelbet")]
		public async ValueTask<CancelBetResult> CancelBet(
			[FromServices] IGetService<CancelBetRequest, CancelBetResult> service,
			CancelBetRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("resettle")]
		public async ValueTask<ResettleResult> Resettle(
			[FromServices] IGetService<ResettleRequest, ResettleResult> service,
			ResettleRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("unsettle")]
		public async ValueTask<UnsettleResult> Unsettle(
			[FromServices] IGetService<UnsettleRequest, UnsettleResult> service,
			UnsettleRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("placebetparlay")]
		public async ValueTask<PlaceBetParlayResult> PlacebetParlay(
			[FromServices] IGetService<PlaceBetParlayRequest, PlaceBetParlayResult> service,
			PlaceBetParlayRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("confirmbetparlay")]
		public async ValueTask<ConfirmBetParlayResult> ConfirmBetParlay(
			[FromServices] IGetService<ConfirmBetParlayRequest, ConfirmBetParlayResult> service,
			ConfirmBetParlayRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("placebet3rd")]
		public async ValueTask<Placebet3rdResult> Placebet3rd(
			[FromServices] IGetService<Placebet3rdRequest, Placebet3rdResult> service,
			Placebet3rdRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("confirmbet3rd")]
		public async ValueTask<ConfirmBet3rdResult> Confirmbet3rd(
				[FromServices] IGetService<ConfirmBet3rdRequest, ConfirmBet3rdResult> service,
				ConfirmBet3rdRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("placebetent")]
		public async ValueTask<PlacebetentResult> PlaceBetEnt(
		[FromServices] IGetService<PlacebetentRequest, PlacebetentResult> service,
		PlacebetentRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("settleent")]
		public async ValueTask<SettleEntResult> SettleEnt(
			[FromServices] IGetService<SettleEntRequest, SettleEntResult> service,
			SettleEntRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("cancelbetent")]
		public async ValueTask<CancelBetEntResult> CancelBetEnt(
			[FromServices] IGetService<CancelBetEntRequest, CancelBetEntResult> service,
			CancelBetEntRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("getticketinfo")]
		public async ValueTask<GetTicketInfoResult> GetTicketInfo(
	[FromServices] IGetService<GetTicketInfoRequest, GetTicketInfoResult> service,
	GetTicketInfoRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("adjustbalance")]
		public async ValueTask<AdjustBalanceResult> AdjustBalance(
[FromServices] IGetService<AdjustBalanceRequest, AdjustBalanceResult> service,
AdjustBalanceRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}
	}
}
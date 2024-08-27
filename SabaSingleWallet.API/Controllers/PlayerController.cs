using Microsoft.AspNetCore.Mvc;
using SabaSingleWallet.API.Models.Balance;
using SabaSingleWallet.API.Models.CancelBet;
using SabaSingleWallet.API.Models.ConfirmBet;
using SabaSingleWallet.API.Models.ConfirmBet3rd;
using SabaSingleWallet.API.Models.ConfirmBetParlay;
using SabaSingleWallet.API.Models.PlaceBet;
using SabaSingleWallet.API.Models.Placebet3rd;
using SabaSingleWallet.API.Models.Placebetent;
using SabaSingleWallet.API.Models.PlaceBetParlay;
using SabaSingleWallet.API.Models.Resettle;
using SabaSingleWallet.API.Models.Settle;
using SabaSingleWallet.API.Models.Unsettle;
using Tgs.ServiceUtility;

namespace SabaSingleWallet.API.Controllers
{
	[Route("api/player")]
	[ApiController]
	public class PlayerController : ControllerBase
	{
		[HttpPost("placebet")]
		public async ValueTask<PlaceBetResult> PlaceBet(
			[FromServices] IGetService<PlaceBetRequest, PlaceBetResult> service,
			PlaceBetRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("getbalance")]
		public async ValueTask<GetBalanceResult> GetBalance(
			[FromServices] IGetService<GetBalanceRequest, GetBalanceResult> service,
			[FromBody] GetBalanceRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("settle")]
		public async ValueTask<SettleResult> Settle(
			[FromServices] IGetService<SettleRequest, SettleResult> service,
			SettleRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}

		[HttpPost("confirmbet")]
		public async ValueTask<ConfirmBetResult> ConfirmBet(
			[FromServices] IGetService<ConfirmBetRequest, ConfirmBetResult> service,
			ConfirmBetRequest source
			)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

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
		public async ValueTask<PlacebetentResult> Placebetent(
		[FromServices] IGetService<PlacebetentRequest, PlacebetentResult> service,
		PlacebetentRequest source)
		{
			var result = await service.GetAsync(source, cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

			return result;
		}
		
	}
}
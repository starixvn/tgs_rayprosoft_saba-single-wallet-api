using Microsoft.AspNetCore.Mvc;
using SabaSingleWallet.API.Models.PlaceBet;
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
    }
}
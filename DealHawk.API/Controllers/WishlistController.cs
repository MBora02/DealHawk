using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.WishlistFavorites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{

    [Authorize]
    public class WishlistController : ApiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetWishlist()
        {
            var query = new GetWishlistQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("toggle")]
        public async Task<ActionResult<bool>> ToggleWishlist([FromBody] ToggleWishlistBody body)
        {
            var command = new ToggleWishlistCommand { GameId = body.GameId, TargetPrice = body.TargetPrice };
            var isWishlisted = await Mediator.Send(command);
            return Ok(isWishlisted);
        }
    }
}

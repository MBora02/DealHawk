using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.WishlistFavorites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    [Authorize]
    public class FavoritesController : ApiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetFavorites()
        {
            var query = new GetFavoritesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("toggle")]
        public async Task<ActionResult<bool>> ToggleFavorite([FromBody] ToggleFavoriteBody body)
        {
            var command = new ToggleFavoriteCommand { GameId = body.GameId };
            var isFavorite = await Mediator.Send(command);
            return Ok(isFavorite);
        }
    }
}

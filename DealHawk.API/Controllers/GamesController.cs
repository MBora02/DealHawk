using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class GamesController : ApiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<PaginatedListDto<GameDto>>> GetGames([FromQuery] string? search, [FromQuery] string? genre, [FromQuery] string? platform, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, [FromQuery] string? sortBy = null, [FromQuery] bool bypassCache = false)
        {
            var query = new GetGamesListQuery
            {
                Search = search,
                Genre = genre,
                Platform = platform,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                BypassCache = bypassCache
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGameById(int id)
        {
            var query = new GetGameDetailsQuery { Id = id };
            var result = await Mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<PriceHistoryDto>>> GetPriceHistory(int id)
        {
            var query = new GetPriceHistoryQuery { GameId = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        public async Task<ActionResult<int>> ImportGames([FromBody] ImportGamesCommand command)
        {
            var count = await Mediator.Send(command);
            return Ok(count);
        }
    }
}

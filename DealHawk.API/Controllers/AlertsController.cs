using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.PriceAlerts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    [Authorize]
    public class AlertsController : ApiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceAlertDto>>> GetUserAlerts()
        {
            var query = new GetUserAlertsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateAlert(CreateAlertCommand command)
        {
            var gameId = await Mediator.Send(command);
            return Ok(gameId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAlert(int id)
        {
            var command = new DeleteAlertCommand { AlertId = id };
            var success = await Mediator.Send(command);
            return Ok(success);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{

    [Authorize]
    public class NotificationsController : ApiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications([FromQuery] bool onlyUnread = false)
        {
            var query = new GetUserNotificationsQuery { OnlyUnread = onlyUnread };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("mark-read")]
        public async Task<ActionResult<bool>> MarkRead([FromBody] MarkReadBody body)
        {
            var command = new MarkNotificationsReadCommand { NotificationId = body.NotificationId };
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}

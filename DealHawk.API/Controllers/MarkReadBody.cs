using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Notifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class MarkReadBody
    {
        public int? NotificationId { get; set; }
    }
}

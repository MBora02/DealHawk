using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Notifications
{
    public class GetUserNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
    {
        public bool OnlyUnread { get; set; }
    }
}

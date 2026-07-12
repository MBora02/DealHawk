using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.Notifications
{
    public class MarkNotificationsReadCommand : IRequest<bool>
    {
        public int? NotificationId { get; set; }
    }
}

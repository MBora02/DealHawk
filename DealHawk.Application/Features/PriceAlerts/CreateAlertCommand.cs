using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.PriceAlerts
{
    public class CreateAlertCommand : IRequest<int>
    {
        public int GameId { get; set; }
        public decimal TargetPrice { get; set; }
    }
}

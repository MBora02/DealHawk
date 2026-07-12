using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.WishlistFavorites
{
    public class ToggleWishlistCommand : IRequest<bool>
    {
        public int GameId { get; set; }
        public decimal? TargetPrice { get; set; }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DealHawk.Application.Features.WishlistFavorites
{
    public class ToggleFavoriteCommand : IRequest<bool>
    {
        public int GameId { get; set; }
    }
}

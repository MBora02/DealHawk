using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.WishlistFavorites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class ToggleWishlistBody
    {
        public int GameId { get; set; }
        public decimal? TargetPrice { get; set; }
    }
}

using System.Collections.Generic;
using DealHawk.Application.DTOs;

namespace DealHawk.WebMVC.Models
{
    public class HomeViewModel
    {
        public IEnumerable<GameDto> TrendingGames { get; set; } = new List<GameDto>();
        public IEnumerable<GameDto> BestDeals { get; set; } = new List<GameDto>();
    }
}

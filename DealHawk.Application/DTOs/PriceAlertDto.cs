using System;

namespace DealHawk.Application.DTOs
{
    public class PriceAlertDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public string GameThumbnailUrl { get; set; }
        public string GameCoverImageUrl { get; set; }
        public decimal TargetPrice { get; set; }
        public decimal? CurrentLowestPrice { get; set; }
        public bool IsTriggered { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

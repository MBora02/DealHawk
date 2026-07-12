using System;

namespace DealHawk.Application.DTOs
{
    public class PriceHistoryDto
    {
        public string StoreName { get; set; }
        public decimal Price { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}

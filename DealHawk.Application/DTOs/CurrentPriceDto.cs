using System;

namespace DealHawk.Application.DTOs
{
    public class CurrentPriceDto
    {
        public string StoreName { get; set; }
        public string StoreLogoUrl { get; set; }
        public decimal Price { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal SavingsPercent { get; set; }
        public string DeepLinkUrl { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

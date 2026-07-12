using System;

namespace DealHawk.Domain.Entities
{

    public class Wishlist
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int GameId { get; set; }

        public decimal? TargetPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; }

        public Game Game { get; set; }
    }
}

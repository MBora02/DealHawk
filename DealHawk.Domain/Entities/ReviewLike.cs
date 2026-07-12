using System;

namespace DealHawk.Domain.Entities
{

    public class ReviewLike
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int ReviewId { get; set; }

        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; }

        public Review Review { get; set; }
    }
}

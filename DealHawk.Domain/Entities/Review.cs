using System;
using System.Collections.Generic;
using DealHawk.Domain.Enums;

namespace DealHawk.Domain.Entities
{

    public class Review
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public int GameId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public DateTime CreatedAt { get; set; }

        public string? ModeratorNotes { get; set; }

        public DateTime? ModeratedAt { get; set; }

        public ApplicationUser User { get; set; }

        public Game Game { get; set; }

        public ICollection<ReviewLike> ReviewLikes { get; set; }
    }
}

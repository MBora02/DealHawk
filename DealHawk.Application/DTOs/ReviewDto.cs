using System;

namespace DealHawk.Application.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public string? ModeratorNotes { get; set; }
        public DateTime? ModeratedAt { get; set; }
    }
}

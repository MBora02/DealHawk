using System;

namespace DealHawk.Application.DTOs
{
    public class ReviewAdminDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } 
        public string UserEmail { get; set; }
        public string UserFullName { get; set; } 
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string? ModeratorNotes { get; set; }
        public DateTime? ModeratedAt { get; set; }
    }
}

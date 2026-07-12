using System;

namespace DealHawk.Application.DTOs
{
    public class FavoriteAdminDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserFullName { get; set; }
        public int GameId { get; set; }
        public string GameTitle { get; set; }
        public string GameThumbnailUrl { get; set; }
        public string GameCoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

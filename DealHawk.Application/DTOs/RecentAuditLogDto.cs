using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class RecentAuditLogDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Action { get; set; } 
        public string Details { get; set; } 
        public DateTime Timestamp { get; set; }
    }
}

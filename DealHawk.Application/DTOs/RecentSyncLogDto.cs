using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class RecentSyncLogDto
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; }
        public int RecordsProcessed { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

using System.Collections.Generic;

namespace DealHawk.Application.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalGames { get; set; }
        public int TotalUsers { get; set; }
        public int TotalActiveAlerts { get; set; }
        public int PendingReviewsCount { get; set; }
        public int ApprovedReviewsCount { get; set; }
        public int RejectedReviewsCount { get; set; }
        public double AverageReviewRating { get; set; }
        public List<StoreDealCountDto> StoreDealCounts { get; set; }
        public List<RecentAuditLogDto> RecentAuditLogs { get; set; }
        public List<RecentSyncLogDto> RecentSyncLogs { get; set; } = new List<RecentSyncLogDto>();
    }
}

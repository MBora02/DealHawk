using Microsoft.EntityFrameworkCore;
using DealHawk.Application.DTOs;
using DealHawk.Application.Interfaces;
using DealHawk.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DealHawk.Persistence.Services
{
    public class AdoNetStatsService : IAdoNetStatsService
    {
        private readonly ApplicationDbContext _context;
        public AdoNetStatsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new DashboardStatsDto();
            var connection = _context.Database.GetDbConnection();
            bool wasOpen = connection.State == ConnectionState.Open;
            try
            {
                if (!wasOpen)
                {
                    await connection.OpenAsync();
                }

                stats.TotalGames = await ExecuteScalarAsync<int>(connection, "SELECT COUNT(*) FROM Games");
                stats.TotalUsers = await ExecuteScalarAsync<int>(connection, "SELECT COUNT(*) FROM AspNetUsers");
                stats.TotalActiveAlerts = await ExecuteScalarAsync<int>(connection, "SELECT COUNT(*) FROM PriceAlerts WHERE IsTriggered = 0");

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Status, COUNT(*) FROM Reviews GROUP BY Status";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int statusVal = reader.GetInt32(0);
                            int count = reader.GetInt32(1);

                            if (statusVal == 0) stats.PendingReviewsCount = count;
                            else if (statusVal == 1) stats.ApprovedReviewsCount = count;
                            else if (statusVal == 2) stats.RejectedReviewsCount = count;
                        }
                    }
                }
                stats.AverageReviewRating = await ExecuteScalarAsync<double>(connection,
                    "SELECT COALESCE(AVG(CAST(Rating AS FLOAT)), 0.0) FROM Reviews WHERE Status = 1");
                stats.StoreDealCounts = new List<StoreDealCountDto>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT s.Name, COUNT(cp.Id)
                        FROM Stores s
                        LEFT JOIN StoreGames sg ON sg.StoreId = s.Id
                        LEFT JOIN CurrentPrices cp ON cp.StoreGameId = sg.Id
                        GROUP BY s.Name";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.StoreDealCounts.Add(new StoreDealCountDto
                            {
                                StoreName = reader.GetString(0),
                                DealCount = reader.GetInt32(1)
                            });
                        }
                    }
                }
                stats.RecentAuditLogs = new List<RecentAuditLogDto>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT TOP 5 al.Id, u.UserName, al.Action, al.Details, al.Timestamp
                        FROM AuditLogs al
                        LEFT JOIN AspNetUsers u ON al.UserId = u.Id
                        ORDER BY al.Timestamp DESC";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.RecentAuditLogs.Add(new RecentAuditLogDto
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.IsDBNull(1) ? "System" : reader.GetString(1),
                                Action = reader.GetString(2),
                                Details = reader.GetString(3),
                                Timestamp = reader.GetDateTime(4)
                            });
                        }
                    }
                }
                stats.RecentSyncLogs = new List<RecentSyncLogDto>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT TOP 5 Id, JobName, StartTime, EndTime, Status, RecordsProcessed, ErrorMessage
                        FROM StoreSyncLogs
                        ORDER BY StartTime DESC";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.RecentSyncLogs.Add(new RecentSyncLogDto
                            {
                                Id = reader.GetInt32(0),
                                JobName = reader.GetString(1),
                                StartTime = reader.GetDateTime(2),
                                EndTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                Status = reader.GetInt32(4) == 0 ? "Running" : (reader.GetInt32(4) == 1 ? "Success" : "Failed"),
                                RecordsProcessed = reader.GetInt32(5),
                                ErrorMessage = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            finally
            {
                if (!wasOpen && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }

            return stats;
        }
        private async Task<T> ExecuteScalarAsync<T>(DbConnection connection, string sql)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return default!;
            }
            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
}

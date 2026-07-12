using System;
using DealHawk.Domain.Enums;

namespace DealHawk.Domain.Entities
{

    public class StoreSyncLog
    {

        public int Id { get; set; }

        public string JobName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public SyncStatus Status { get; set; } = SyncStatus.Running;

        public int RecordsProcessed { get; set; }

        public string? ErrorMessage { get; set; }
    }
}

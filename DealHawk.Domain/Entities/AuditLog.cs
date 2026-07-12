using System;

namespace DealHawk.Domain.Entities
{

    public class AuditLog
    {

        public int Id { get; set; }

        public string? UserId { get; set; }

        public string Action { get; set; }

        public string Details { get; set; }

        public DateTime Timestamp { get; set; }

        public string? IpAddress { get; set; }

        public ApplicationUser? User { get; set; }
    }
}

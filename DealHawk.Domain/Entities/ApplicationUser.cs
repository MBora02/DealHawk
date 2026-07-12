using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DealHawk.Domain.Entities
{

    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; }

        public ICollection<Favorite> Favorites { get; set; }

        public ICollection<Wishlist> Wishlists { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<ReviewLike> ReviewLikes { get; set; }

        public ICollection<PriceAlert> PriceAlerts { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}

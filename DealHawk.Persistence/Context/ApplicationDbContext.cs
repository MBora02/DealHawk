using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DealHawk.Domain.Entities;

namespace DealHawk.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Publisher> Publishers => Set<Publisher>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<GameGenre> GameGenres => Set<GameGenre>();
        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<StoreGame> StoreGames => Set<StoreGame>();
        public DbSet<CurrentPrice> CurrentPrices => Set<CurrentPrice>();
        public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewLike> ReviewLikes => Set<ReviewLike>();
        public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<StoreSyncLog> StoreSyncLogs => Set<StoreSyncLog>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<GameGenre>()
                .HasKey(gg => new { gg.GameId, gg.GenreId });

            builder.Entity<GameGenre>()
                .HasOne(gg => gg.Game)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GameId);

            builder.Entity<GameGenre>()
                .HasOne(gg => gg.Genre)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GenreId);

            builder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GameId, gp.PlatformId });

            builder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GameId);

            builder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(p => p.GamePlatforms)
                .HasForeignKey(gp => gp.PlatformId);

            builder.Entity<CurrentPrice>()
                .Property(cp => cp.Price)
                .HasPrecision(18, 2);

            builder.Entity<CurrentPrice>()
                .Property(cp => cp.RetailPrice)
                .HasPrecision(18, 2);

            builder.Entity<CurrentPrice>()
                .Property(cp => cp.SavingsPercent)
                .HasPrecision(18, 2);

            builder.Entity<PriceHistory>()
                .Property(ph => ph.Price)
                .HasPrecision(18, 2);

            builder.Entity<Wishlist>()
                .Property(w => w.TargetPrice)
                .HasPrecision(18, 2);

            builder.Entity<PriceAlert>()
                .Property(pa => pa.TargetPrice)
                .HasPrecision(18, 2);

            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReviewLike>()
                .HasOne(rl => rl.User)
                .WithMany(u => u.ReviewLikes)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PriceAlert>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.PriceAlerts)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

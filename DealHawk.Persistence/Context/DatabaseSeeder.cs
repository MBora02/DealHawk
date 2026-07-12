using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using DealHawk.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealHawk.Persistence.Context
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            var roles = new[] { "Admin", "Moderator", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@dealhawk.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var modEmail = "moderator@dealhawk.com";
            var modUser = await userManager.FindByEmailAsync(modEmail);
            if (modUser == null)
            {
                modUser = new ApplicationUser
                {
                    UserName = "moderator",
                    Email = modEmail,
                    FullName = "Content Moderator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(modUser, "Moderator123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(modUser, "Moderator");
                }
            }

            var userEmail = "user@dealhawk.com";
            var standardUser = await userManager.FindByEmailAsync(userEmail);
            if (standardUser == null)
            {
                standardUser = new ApplicationUser
                {
                    UserName = "user",
                    Email = userEmail,
                    FullName = "Regular User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(standardUser, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(standardUser, "User");
                }
            }

            if (!context.Stores.Any())
            {
                var storesToSeed = new List<Store>
                {
                    new Store { Name = "Steam", CheapSharkStoreId = "1", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/0.png" },
                    new Store { Name = "GreenManGaming", CheapSharkStoreId = "2", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/1.png" },
                    new Store { Name = "Fanatical", CheapSharkStoreId = "3", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/2.png" },
                    new Store { Name = "GOG", CheapSharkStoreId = "7", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/6.png" },
                    new Store { Name = "Humble Store", CheapSharkStoreId = "11", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/10.png" },
                    new Store { Name = "Epic Games Store", CheapSharkStoreId = "25", IsActive = true, LogoUrl = "https://www.cheapshark.com/img/stores/logos/24.png" }
                };
                await context.Stores.AddRangeAsync(storesToSeed);
            }

            if (!context.Genres.Any())
            {
                var genresToSeed = new List<Genre>
                {
                    new Genre { Name = "Action" },
                    new Genre { Name = "RPG" },
                    new Genre { Name = "Adventure" },
                    new Genre { Name = "Strategy" },
                    new Genre { Name = "Shooter" },
                    new Genre { Name = "Indie" }
                };
                await context.Genres.AddRangeAsync(genresToSeed);
            }

            if (!context.Platforms.Any())
            {
                var platformsToSeed = new List<Platform>
                {
                    new Platform { Name = "PC" },
                    new Platform { Name = "PlayStation" },
                    new Platform { Name = "Xbox" },
                    new Platform { Name = "Nintendo Switch" }
                };
                await context.Platforms.AddRangeAsync(platformsToSeed);
            }

            if (!context.Publishers.Any())
            {
                var publishersToSeed = new List<Publisher>
                {
                    new Publisher { Name = "FromSoftware" },
                    new Publisher { Name = "CD Projekt Red" },
                    new Publisher { Name = "Xbox Game Studios" },
                    new Publisher { Name = "Sony Interactive Entertainment" },
                    new Publisher { Name = "Electronic Arts" },
                    new Publisher { Name = "Ubisoft" }
                };
                await context.Publishers.AddRangeAsync(publishersToSeed);
            }

            await context.SaveChangesAsync();
        }
    }
}

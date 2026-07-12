using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Dashboard;
using DealHawk.Application.Interfaces;
using DealHawk.Domain.Entities;
using DealHawk.Infrastructure.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ApiControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ICheapSharkService _cheapSharkService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(IUnitOfWork unitOfWork, IBackgroundJobClient backgroundJobClient, ICheapSharkService cheapSharkService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
            _cheapSharkService = cheapSharkService;
            _userManager = userManager;
        }
        [HttpPost("import-bulk")]
        public ActionResult ImportBulkGames([FromQuery] int count = 100)
        {
            _backgroundJobClient.Enqueue<ICheapSharkService>(service => service.ImportBulkGamesAsync(count));
            return Ok();
        }
        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var result = await Mediator.Send(new GetAdminDashboardStatsQuery());
            return Ok(result);
        }
        [HttpPost("sync")]
        public ActionResult TriggerSync()
        {
            var jobId = _backgroundJobClient.Enqueue<PriceSyncJob>(job => job.RunSyncJobAsync());
            return Ok(new { Message = "CheapShark price synchronization job queued.", JobId = jobId });
        }
        [HttpGet("audit-logs")]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
        {
            var logs = await _unitOfWork.Repository<AuditLog>().Query()
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Take(100)
                .ToListAsync();

            var result = logs.Select(a => new
            {
                a.Id,
                Username = a.User?.UserName ?? "System",
                a.Action,
                a.Details,
                a.Timestamp,
                a.IpAddress
            });

            return Ok(result);
        }
        [HttpGet("sync-logs")]
        public async Task<ActionResult<IEnumerable<StoreSyncLog>>> GetSyncLogs()
        {
            var logs = await _unitOfWork.Repository<StoreSyncLog>().Query()
                .OrderByDescending(a => a.StartTime)
                .Take(50)
                .ToListAsync();

            return Ok(logs);
        }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserAdminDto>>> GetUsers()
        {
            var users = await _unitOfWork.Repository<ApplicationUser>().Query().ToListAsync();
            var result = new List<UserAdminDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserAdminDto
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    FullName = u.FullName,
                    Roles = roles.ToList()
                });
            }
            return Ok(result);
        }

        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<GenreAdminDto>>> GetGenres()
        {
            var genres = await _unitOfWork.Repository<Genre>().Query()
                .Select(g => new GenreAdminDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    GameCount = g.GameGenres.Count
                })
                .ToListAsync();
            return Ok(genres);
        }

        [HttpGet("stores")]
        public async Task<ActionResult<IEnumerable<StoreAdminDto>>> GetStores()
        {
            var stores = await _unitOfWork.Repository<Store>().Query()
                .Select(s => new StoreAdminDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CheapSharkStoreId = s.CheapSharkStoreId,
                    IsActive = s.IsActive,
                    LogoUrl = s.LogoUrl,
                    GameCount = s.StoreGames.Count
                })
                .ToListAsync();
            return Ok(stores);
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<IEnumerable<FavoriteAdminDto>>> GetFavorites()
        {
            var favorites = await _unitOfWork.Repository<Favorite>().Query()
                .Select(f => new FavoriteAdminDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    UserName = f.User.UserName ?? "Unknown",
                    UserEmail = f.User.Email ?? "Unknown",
                    UserFullName = f.User.FullName,
                    GameId = f.GameId,
                    GameTitle = f.Game.Title,
                    GameThumbnailUrl = f.Game.ThumbnailUrl,
                    GameCoverImageUrl = !string.IsNullOrEmpty(f.Game.SteamAppId) && f.Game.SteamAppId != "0" && !string.Equals(f.Game.SteamAppId, "null", StringComparison.OrdinalIgnoreCase)
                        ? $"https://cdn.akamai.steamstatic.com/steam/apps/{f.Game.SteamAppId}/header.jpg"
                        : (string.IsNullOrEmpty(f.Game.ThumbnailUrl)
                            ? "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=400&q=80"
                            : f.Game.ThumbnailUrl),
                    CreatedAt = f.CreatedAt
                })
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
            return Ok(favorites);
        }

        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<ReviewAdminDto>>> GetReviews()
        {
            var reviews = await _unitOfWork.Repository<Review>().Query()
                .Select(r => new ReviewAdminDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserName = r.User.UserName ?? "Unknown",
                    UserEmail = r.User.Email ?? "Unknown",
                    UserFullName = r.User.FullName,
                    GameId = r.GameId,
                    GameTitle = r.Game.Title,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    ModeratorNotes = r.ModeratorNotes,
                    ModeratedAt = r.ModeratedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return Ok(reviews);
        }

        [HttpDelete("games/{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _unitOfWork.Repository<Game>().Query()
                .Include(g => g.GameGenres)
                .Include(g => g.GamePlatforms)
                .Include(g => g.Favorites)
                .Include(g => g.Wishlists)
                .Include(g => g.PriceAlerts)
                .Include(g => g.Reviews).ThenInclude(r => r.ReviewLikes)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.CurrentPrices)
                .Include(g => g.StoreGames).ThenInclude(sg => sg.PriceHistories)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {

                foreach (var gg in game.GameGenres)
                {
                    _unitOfWork.Repository<GameGenre>().Delete(gg);
                }

                foreach (var gp in game.GamePlatforms)
                {
                    _unitOfWork.Repository<GamePlatform>().Delete(gp);
                }

                foreach (var fav in game.Favorites)
                {
                    _unitOfWork.Repository<Favorite>().Delete(fav);
                }

                foreach (var wish in game.Wishlists)
                {
                    _unitOfWork.Repository<Wishlist>().Delete(wish);
                }

                foreach (var alert in game.PriceAlerts)
                {
                    _unitOfWork.Repository<PriceAlert>().Delete(alert);
                }

                foreach (var review in game.Reviews)
                {
                    foreach (var like in review.ReviewLikes)
                    {
                        _unitOfWork.Repository<ReviewLike>().Delete(like);
                    }
                    _unitOfWork.Repository<Review>().Delete(review);
                }

                foreach (var sg in game.StoreGames)
                {
                    foreach (var cp in sg.CurrentPrices)
                    {
                        _unitOfWork.Repository<CurrentPrice>().Delete(cp);
                    }
                    foreach (var ph in sg.PriceHistories)
                    {
                        _unitOfWork.Repository<PriceHistory>().Delete(ph);
                    }
                    _unitOfWork.Repository<StoreGame>().Delete(sg);
                }

                _unitOfWork.Repository<Game>().Delete(game);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Ok(new { Success = true, Message = "Game deleted successfully." });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { Message = "An error occurred while deleting the game.", Details = ex.Message });
            }
        }
    }
}

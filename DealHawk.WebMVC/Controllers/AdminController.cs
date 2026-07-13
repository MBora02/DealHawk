using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public AdminController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = await _apiClient.GetDashboardStatsAsync();
                var auditLogs = await _apiClient.GetAuditLogsAsync();
                var syncLogs = await _apiClient.GetSyncLogsAsync();

                ViewBag.Stats = stats;
                ViewBag.AuditLogs = auditLogs;
                ViewBag.SyncLogs = syncLogs;

                return View();
            }
            catch (System.Net.Http.HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Logout", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> TriggerSync()
        {
            var success = await _apiClient.TriggerSyncAsync();
            if (success)
            {
                TempData["Success"] = "Database synchronization job enqueued in background.";
            }
            else
            {
                TempData["Error"] = "Failed to trigger database synchronization.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ImportGames(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                TempData["Error"] = "Please enter a game title to import.";
                return RedirectToAction("Index");
            }

            var count = await _apiClient.ImportGamesAsync(title);
            if (count > 0)
            {
                TempData["Success"] = $"Successfully imported {count} games matching '{title}'!";
            }
            else
            {
                TempData["Error"] = $"No new games found or imported for '{title}'.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ImportBulk(int count = 25)
        {
            var enqueued = await _apiClient.ImportBulkGamesAsync(count);
            if (enqueued)
            {
                TempData["Success"] = $"Bulk import of {count} games enqueued in the background! Games will appear in the catalog shortly.";
            }
            else
            {
                TempData["Error"] = "Failed to queue bulk games import.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _apiClient.GetUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Games(string? search, int pageNumber = 1)
        {
            var result = await _apiClient.GetGamesAsync(search: search, pageNumber: pageNumber, pageSize: 15);
            ViewBag.Search = search;
            return View(result);
        }

        public async Task<IActionResult> Genres()
        {
            var genres = await _apiClient.GetSystemGenresAsync();
            return View(genres);
        }

        public async Task<IActionResult> Stores()
        {
            var stores = await _apiClient.GetSystemStoresAsync();
            return View(stores);
        }

        public async Task<IActionResult> Favorites()
        {
            var favorites = await _apiClient.GetSystemFavoritesAsync();
            return View(favorites);
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _apiClient.GetSystemReviewsAsync();
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var success = await _apiClient.DeleteGameAsync(id);
            if (success)
            {
                TempData["Success"] = "Game and all its related references (reviews, favorites, price alerts) were deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete the game.";
            }
            return RedirectToAction("Games");
        }
    }
}

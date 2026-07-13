using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public DashboardController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var favorites = await _apiClient.GetFavoritesAsync();
                var wishlist = await _apiClient.GetWishlistAsync();
                var alerts = await _apiClient.GetUserAlertsAsync();
                var notifications = await _apiClient.GetNotificationsAsync();

                ViewBag.Favorites = favorites;
                ViewBag.Wishlist = wishlist;
                ViewBag.Alerts = alerts;
                ViewBag.Notifications = notifications;

                return View();
            }
            catch (System.Net.Http.HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Logout", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int gameId)
        {
            await _apiClient.ToggleFavoriteAsync(gameId);
            return Redirect("/Dashboard#favorites");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavoriteFromDetails(int gameId)
        {
            await _apiClient.ToggleFavoriteAsync(gameId);
            return RedirectToAction("Details", "Games", new { id = gameId });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(int gameId, decimal? targetPrice)
        {
            await _apiClient.ToggleWishlistAsync(gameId, targetPrice);
            return Redirect("/Dashboard#wishlist");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleWishlistFromDetails(int gameId, decimal? targetPrice)
        {
            await _apiClient.ToggleWishlistAsync(gameId, targetPrice);
            return RedirectToAction("Details", "Games", new { id = gameId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlert(int gameId, decimal targetPrice)
        {
            if (targetPrice <= 0)
            {
                TempData["Error"] = "Target price must be greater than $0.00.";
                return RedirectToAction("Details", "Games", new { id = gameId });
            }

            var alertId = await _apiClient.CreateAlertAsync(gameId, targetPrice);
            if (alertId > 0)
            {
                TempData["Success"] = $"Price alert configured successfully at ${targetPrice:F2}!";
            }
            else
            {
                TempData["Error"] = "Failed to configure price alert.";
            }

            return RedirectToAction("Details", "Games", new { id = gameId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAlert(int alertId)
        {
            await _apiClient.DeleteAlertAsync(alertId);
            return Redirect("/Dashboard#alerts");
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int? notificationId)
        {
            await _apiClient.MarkNotificationsReadAsync(notificationId);
            return Redirect("/Dashboard#notifications");
        }
    }
}

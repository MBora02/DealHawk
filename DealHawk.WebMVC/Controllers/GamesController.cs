using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    public class GamesController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public GamesController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index(string? search, string? genre, string? platform, int page = 1)
        {
            var games = await _apiClient.GetGamesAsync(search, genre, platform, pageNumber: page, pageSize: 20);
            ViewBag.Search = search;
            ViewBag.Genre = genre;
            ViewBag.Platform = platform;
            ViewBag.PageNumber = page;
            return View(games);
        }

        public async Task<IActionResult> Details(int id)
        {
            var game = await _apiClient.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var priceHistory = await _apiClient.GetPriceHistoryAsync(id);
            var reviews = await _apiClient.GetGameReviewsAsync(id);

            ViewBag.PriceHistory = priceHistory;
            ViewBag.Reviews = reviews;

            return View(game);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitReview(int gameId, int rating, string comment)
        {
            if (rating < 1 || rating > 5 || string.IsNullOrEmpty(comment))
            {
                TempData["Error"] = "Please provide a valid rating (1-5) and comment.";
                return RedirectToAction("Details", new { id = gameId });
            }

            var reviewId = await _apiClient.SubmitReviewAsync(gameId, rating, comment);
            if (reviewId > 0)
            {
                TempData["Success"] = "Review submitted! It will appear once approved by a moderator.";
            }
            else
            {
                TempData["Error"] = "Failed to submit review.";
            }

            return RedirectToAction("Details", new { id = gameId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeReview(int reviewId, int gameId)
        {
            await _apiClient.LikeReviewAsync(reviewId);
            return RedirectToAction("Details", new { id = gameId });
        }
    }
}

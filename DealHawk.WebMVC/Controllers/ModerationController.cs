using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.WebMVC.Services;
using System.Threading.Tasks;

namespace DealHawk.WebMVC.Controllers
{

    [Authorize(Roles = "Admin,Moderator")]
    public class ModerationController : Controller
    {
        private readonly DealHawkApiClient _apiClient;

        public ModerationController(DealHawkApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var pendingReviews = await _apiClient.GetPendingReviewsAsync();
            return View(pendingReviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int reviewId, string? notes)
        {
            var success = await _apiClient.ApproveReviewAsync(reviewId, notes);
            if (success)
            {
                TempData["Success"] = $"Review ID {reviewId} approved.";
            }
            else
            {
                TempData["Error"] = "Failed to approve review.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int reviewId, string? notes)
        {
            var success = await _apiClient.RejectReviewAsync(reviewId, notes);
            if (success)
            {
                TempData["Success"] = $"Review ID {reviewId} rejected.";
            }
            else
            {
                TempData["Error"] = "Failed to reject review.";
            }
            return RedirectToAction("Index");
        }
    }
}

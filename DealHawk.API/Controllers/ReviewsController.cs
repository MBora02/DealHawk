using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DealHawk.Application.DTOs;
using DealHawk.Application.Features.Reviews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DealHawk.API.Controllers
{
    public class ReviewsController : ApiControllerBase
    {

        [HttpGet("game/{gameId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForGame(int gameId)
        {
            var query = new GetGameReviewsQuery { GameId = gameId };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<int>> SubmitReview(SubmitReviewCommand command)
        {
            var reviewId = await Mediator.Send(command);
            return Ok(reviewId);
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<ActionResult<bool>> LikeReview(int id)
        {
            var command = new LikeReviewCommand { ReviewId = id };
            var liked = await Mediator.Send(command);
            return Ok(liked);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetPendingReviews()
        {
            var query = new GetPendingReviewsQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{id}/approve")]
        public async Task<ActionResult<bool>> ApproveReview(int id, [FromBody] ApproveReviewBody body)
        {
            var command = new ApproveReviewCommand { ReviewId = id, ModeratorNotes = body.ModeratorNotes };
            var success = await Mediator.Send(command);
            return Ok(success);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{id}/reject")]
        public async Task<ActionResult<bool>> RejectReview(int id, [FromBody] RejectReviewBody body)
        {
            var command = new RejectReviewCommand { ReviewId = id, ModeratorNotes = body.ModeratorNotes };
            var success = await Mediator.Send(command);
            return Ok(success);
        }
    }
}

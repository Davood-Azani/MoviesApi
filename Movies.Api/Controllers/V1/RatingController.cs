using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers.V1
{

    [ApiController]
    public class RatingController(IRatingService ratingService) : ControllerBase
    {
        [Authorize]
        [HttpPut(ApiEndpoints.V1.Movies.Rate)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var res = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
            return res ? Ok() : NotFound();
        }
        [Authorize]
        [HttpDelete(ApiEndpoints.V1.Movies.DeleteRating)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var res = await ratingService.DeleteRatingAsync(id, userId!.Value, token);
            return res ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.V1.Ratings.GetUserRatings)]
        public async Task<IActionResult> GetUserRatings(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movieRatings = await ratingService.GetRatingsFoUserAsync(userId!.Value, token);

            var res = movieRatings.MapToMovieRatingResponse();
            return Ok(res);
        }


    }
}

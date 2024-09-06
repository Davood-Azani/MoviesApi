using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{

    [ApiController]
    [ApiVersion(1.0)]
    public class RatingController(IRatingService ratingService) : ControllerBase
    {
        [Authorize]
        [HttpPut(ApiEndpoints.Movies.Rate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RateMovie([FromRoute] Guid id, [FromBody] RateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var res = await ratingService.RateMovieAsync(id, request.Rating, userId!.Value, token);
            return res ? Ok() : NotFound();
        }
        [Authorize]
        [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var res = await ratingService.DeleteRatingAsync(id, userId!.Value, token);
            return res ? Ok() : NotFound();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
        [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetUserRatings(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movieRatings = await ratingService.GetRatingsFoUserAsync(userId!.Value, token);

            var res = movieRatings.MapToMovieRatingResponse();
            return Ok(res);
        }


    }
}

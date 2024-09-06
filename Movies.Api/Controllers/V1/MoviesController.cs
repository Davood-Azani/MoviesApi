using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers.V1
{

    [ApiController]

    public class MoviesController(IMovieService movieService) : ControllerBase
    {
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.V1.Movies.Create)] // api/movies
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {

            var movie = request.MapToMovie();
            await movieService.CreateAsync(movie, token);
            //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
            // return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
        [Authorize]
        [HttpGet(ApiEndpoints.V1.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token
            //,[FromServices] LinkGenerator linkGenerator
            )
        {
            var userId = HttpContext.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await movieService.GetByIdAsync(id, userId, token)
                : await movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie == null)
            {
                return NotFound();
            }
            var response = movie.MapToMovieResponse();

            #region Heatos
            //var movieObj = new { id = movie.Id };

            //response.Links.Add(new Link
            //{

            //    Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get),  values: new
            //    {
            //        idOrSlug = movie.Id

            //    }),
            //    Rel = "self", // means that the link is pointing to the current resource
            //    Type = "GET"



            //});
            //response.Links.Add(new Link
            //{

            //    Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update),  values:movieObj),
            //    Rel = "self", // means that the link is pointing to the current resource
            //    Type = "PUT"



            //}); 
            //response.Links.Add(new Link
            //{

            //    Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete),  values: new
            //    {
            //        id = movie.Id

            //    }),
            //    Rel = "self", // means that the link is pointing to the current resource
            //    Type = "DELETE"



            //}); 
            #endregion


            return Ok(response);
        }
        [Authorize]
        [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
        public async Task<IActionResult> GetMovies([FromQuery] GetAllMoviesRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var options = request.MapToOptions().WithUser(userId);
            var movies = await movieService.GetAllAsync(options, token);
            var moviesCount = await movieService.GetCountAsync(request.Title, request.Year, token);



            var response = movies.MapToMoviesResponse(request.Page, request.PageSize, moviesCount);
            return Ok(response);
        }
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.V1.Movies.Update)] // api/movies/{id} |  for full update
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = request.MapToMovie(id);
            var updateMovie = await movieService.UpdateAsync(movie, userId, token);
            if (updateMovie is null) return NotFound();

            var response = movie.MapToMovieResponse();

            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var res = await movieService.DeleteByIdAsync(id, token);
            if (!res) return NotFound();
            return NoContent();
        }
    }
}

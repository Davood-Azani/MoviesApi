using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers
{

    [ApiController]
    [ApiVersion(1.0)]
    //[ApiVersion(2.0)]
    public class MoviesController(IMovieService movieService, IOutputCacheStore cacheStore) : ControllerBase
    {
        //   [MapToApiVersion(1.0)]
        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)] // api/movies

        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)] // for swagger documentation
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)] // for swagger documentation
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {

            var movie = request.MapToMovie();
            await movieService.CreateAsync(movie, token);

            await cacheStore.EvictByTagAsync("movie", token); // evict the cache for the movie tag means
                                                              // that the cache will be invalidated and the next request will be fetched from the database

            //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
            // return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
        //[MapToApiVersion(2.0)]
        //[Authorize(AuthConstants.TrustedMemberPolicyName)]
        //[HttpPost(ApiEndpoints.Movies.Create)] // api/movies
        //public async Task<IActionResult> CreateV2([FromBody] CreateMovieRequest request, CancellationToken token)
        //{

        //    var movie = request.MapToMovie();
        //    await movieService.CreateAsync(movie, token);
        //    //return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
        //    // return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        //    return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        //}









        // [ApiVersion(1.0)]
        //[Authorize]
        [HttpGet(ApiEndpoints.Movies.Get)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)] // for swagger documentation
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ResponseCache(Duration = 30, VaryByQueryKeys = ["title", "year", "sortBy", "pageSize"],VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
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

        // [Authorize]
        //  [HttpGet(ApiEndpoints.Movies.Get)]
        //  [ApiVersion(2.0)]
        //public async Task<IActionResult> GetV2([FromRoute] string idOrSlug, CancellationToken token
        //    //,[FromServices] LinkGenerator linkGenerator
        //)
        //{
        //    var userId = HttpContext.GetUserId();
        //    var movie = Guid.TryParse(idOrSlug, out var id)
        //        ? await movieService.GetByIdAsync(id, userId, token)
        //        : await movieService.GetBySlugAsync(idOrSlug, userId, token);
        //    if (movie == null)
        //    {
        //        return NotFound();
        //    }
        //    var response = movie.MapToMovieResponse();


        //    return Ok(response);
        //}


         [Authorize]
        // [ResponseCache(Duration = 30 ,  VaryByHeader = "Accept, Accept-Encoding" , Location = ResponseCacheLocation.Any)]
        [OutputCache(PolicyName = "MovieCache")]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)] // for swagger documentation
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
        [HttpPut(ApiEndpoints.Movies.Update)] // api/movies/{id} |  for full update
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)] // for swagger documentation
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)] // for swagger documentation
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = request.MapToMovie(id);
            var updateMovie = await movieService.UpdateAsync(movie, userId, token);
            if (updateMovie is null) return NotFound();
            await cacheStore.EvictByTagAsync("movie", token);
            var response = movie.MapToMovieResponse();

            return Ok(response);
        }

         [Authorize(AuthConstants.AdminUserPolicyName)]
        // [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var res = await movieService.DeleteByIdAsync(id, token);
            if (!res) return NotFound();
            await cacheStore.EvictByTagAsync("movie", token);
            return NoContent();
        }
    }
}

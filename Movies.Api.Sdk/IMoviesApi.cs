using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

[Headers("Authorization: Bearer")]
public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    // it's the same as [HttpGet(ApiEndpoints.Movies.Get)] in the controller
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
    // it's the same as Task<IActionResult> GetMovie(string idOrSlug)

    [Get(ApiEndpoints.Movies.GetAll)] // api/movies
    Task<MoviesResponse> GetAllMoviesAsync(GetAllMoviesRequest request);
    // it's the same as Task<IActionResult> GetAllMovies()

    [Post(ApiEndpoints.Movies.Create)] // api/movies
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);


    [Put(ApiEndpoints.Movies.Update)] // api/movies
    Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);


    [Delete(ApiEndpoints.Movies.Delete)] // api/movies
    Task DeleteMovieAsync(Guid id);

    [Put(ApiEndpoints.Movies.Rate)] // api/movies/rate
    Task RateMovieAsync(Guid id, RateMovieRequest request);

    [Delete(ApiEndpoints.Movies.DeleteRating)] // api/movies/rate
    Task DeleteRatingAsync(Guid id);

    [Get(ApiEndpoints.Ratings.GetUserRatings)] // api/ratings
    Task<IEnumerable<MovieRatingResponse>> GetUserRatingsAsync();

}
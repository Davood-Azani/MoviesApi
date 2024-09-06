using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.MinimalApi.Endpoints.Movies;


public static class GetMovieEndpoint
{

    public const string Name = "GetMovie";
    public static void MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            ApiEndpoints.Movies.Get,// this is the endpoint url
            async (string idOrSlug, IMovieService movieService,HttpContext context, CancellationToken token) => // our endpoint handler or action
        {

            var userId = context.GetUserId();
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await movieService.GetByIdAsync(id, userId, token)
                : await movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie == null)
            {
                return Results.NotFound();
            }
            var response = movie.MapToMovieResponse();
            return TypedResults.Ok(response);
        });
    }
}

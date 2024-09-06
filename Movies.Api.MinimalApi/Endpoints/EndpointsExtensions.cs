using Movies.Api.MinimalApi.Endpoints.Movies;
using Movies.Api.MinimalApi.Endpoints.Ratings;

namespace Movies.Api.MinimalApi.Endpoints;

public static class EndpointsExtensions
{

    public static IEndpointRouteBuilder MapApiEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapMovieEndpoints();
        app.MapRatingEndpoints();
        
        return app;
    }
}

using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository,
    IValidator<Movie> validator ,  IRatingRepository ratingRepository , IValidator<GetAllMoviesOptions> optionsValidator) : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token)
    {
        await validator.ValidateAndThrowAsync(movie, cancellationToken: token);

        return await movieRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        return movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        return movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {

        await optionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);
        
        return await movieRepository.GetAllAsync(options,token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var existedMovie = await movieRepository.ExistByIdAsync(movie.Id, token);
        if (!existedMovie)
        {
            return null;
        }
        await movieRepository.UpdateAsync(movie, token);

        if (!userId.HasValue)
        {
            var rating = await ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
        }

        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId!.Value, token);
        movie.UserRating = ratings.UserRating;
        movie.Rating = ratings.Rating;
        
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token)
    {
        return movieRepository.DeleteByIdAsync(id, token);
    }

    public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        return movieRepository.GetCountAsync(title, yearOfRelease, token);
    }
}
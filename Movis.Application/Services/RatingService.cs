

using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository , IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        //if (rating is <= 0 || rating > 5)
        if (rating is <= 0 or > 5)
        {
            throw new ValidationException(
            [
                new ValidationFailure("rating", "Rating must be between 1 and 5")
                
            ]);
        }

        var movie = await movieRepository.ExistByIdAsync(movieId, token);
        if (!movie) return false;
        //if (!movie)
        //{
        //    throw new ValidationException(new[]
        //    {
        //        new ValidationFailure("movie", "Movie not found")
        //    });
        //}
        
        var res = await ratingRepository.RateMovieAsync(movieId, rating, userId, token);
        return res;
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
       return ratingRepository.DeleteRatingAsync(movieId, userId, token); // we call it forwading 
    }

    public Task<IEnumerable<MovieRating>> GetRatingsFoUserAsync(Guid userId, CancellationToken token = default)
    {
        return ratingRepository.GetRatingsFoUserAsync(userId, token);
    }
}


using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Validators;

public class MovieValidators : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    // we use IMovieService because this and IMovieService 
    // are at the same the level And we don't want to have a circular dependency
    public MovieValidators(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty()
            ;
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This Movie already exists");
        }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existedMovie = await _movieRepository.GetBySlugAsync(slug);

        if (existedMovie is not null)
        {
            return existedMovie.Id == movie.Id;
        }
        return existedMovie is null;
    }
}
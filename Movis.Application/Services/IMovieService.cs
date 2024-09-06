using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default);
    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options , CancellationToken token = default);
    Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default);

}

// if we use dto in repository here we should be using dto in service as well ex :
// Task<bool> CreateAsync(MovieDto movie);
// Task<MovieDto?> GetByIdAsync(Guid id);
// and we need to use mapper to convert in the application layer
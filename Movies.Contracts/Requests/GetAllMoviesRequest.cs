

namespace Movies.Contracts.Requests;

public class GetAllMoviesRequest : PagedRequest // for pagination
{

    // filter
    public required string? Title { get; init; }

    public required int? Year{ get; init; }

    // sorting
    public required string? SortBy { get; init; }


    



}
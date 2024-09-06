

using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    // we use init as id should not change during application run time
    public required string Title { get; set; }
    // we use set as title can change during application  run time



    public float? Rating { get; set; } // the average rating of the movie
    // we use set as rating can change during application  run time
    public int? UserRating { get; set; }
    // we use set as user rating can change during application  run time


    public required int YearOfRelease { get; set; }
    // we use set as year of release can change during application  run time

    public string Slug => GenerateSlug();


    public required List<string> Genres { get; init; } = [];
    // we use init as genres should not change during application run time
    // list is mutable so we can use new() to initialize it

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty).ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";


    }

    [GeneratedRegex(@"[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
    //private string GenerateSlug()
    //{
    //    var sluggedTitle = Regex.Replace(Title, @"[^0-9A-Za-z _-]", string.Empty).ToLower().Replace(" " , "-");
    //    return $"{sluggedTitle}-{YearOfRelease}";


    //}
}
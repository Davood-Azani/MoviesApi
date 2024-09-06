

using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{


    private static readonly string[] AllowedSortFields = { "title", "yearofrelease" };


    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x=>x.SortField)
            .Must(x => x is null || AllowedSortFields.Contains(x.ToLower() , StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Sort field must be one of the following: {string.Join(", ", AllowedSortFields)}");


        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("Page size must be between 1 and 25");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

    }
}
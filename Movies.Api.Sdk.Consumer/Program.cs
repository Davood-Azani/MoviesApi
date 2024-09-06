// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;

Console.WriteLine("Hello, World!");

//var movieApi = RestService.For<IMoviesApi>("https://localhost:7293");

#region Get A Movie
// get a movie
//var movie = await movieApi.GetMovieAsync("e2e45903-94f9-4979-bfdf-b1de7b36a874");
//Console.WriteLine(JsonSerializer.Serialize(movie));

#endregion


#region Get All Movies
// get all movies
//var movies = await movieApi.GetAllMoviesAsync(new GetAllMoviesRequest
//{
//    Page = 1,
//    PageSize = 2,
//    Title = null,
//    Year = null,
//    SortBy = null

//});


//foreach (var movieObj in movies.Items)
//{

//    Console.WriteLine(JsonSerializer.Serialize(movies));
//}
//Console.WriteLine(movies.Page);
//Console.WriteLine(movies.HasNextPage);
//Console.WriteLine(movies.Total);
//

#endregion


// HttpClient and DependencyInjection

var services = new ServiceCollection();
#region Sttaic-Token
//services.AddRefitClient<IMoviesApi>(x => new RefitSettings
//{

//    AuthorizationHeaderValueGetter = (message, token) =>
//        Task.FromResult("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5ZDM3ZDZhYi0yMTcyLTRiYzMtODFmMC1jYjJkNjEwYWM2MGUiLCJzdWIiOiJuaWNrQG5pY2tjaGFwc2FzLmNvbSIsImVtYWlsIjoibmlja0BuaWNrY2hhcHNhcy5jb20iLCJ1c2VyaWQiOiJkODU2NmRlMy1iMWE2LTRhOWItYjg0Mi04ZTM4ODdhODJlNDEiLCJhZG1pbiI6dHJ1ZSwidHJ1c3RlZF9tZW1iZXIiOmZhbHNlLCJuYmYiOjE3MjU1MTk1OTYsImV4cCI6MTcyNTU0ODM5NiwiaWF0IjoxNzI1NTE5NTk2LCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMDMiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjcyOTMifQ.6iA0o4EG3CdECFkT1JWMNExGUGFDhuLyB549ru1I9_Y"), // add the bearer token here


//}) 
#endregion
#region Dynamic-Token
services
.AddHttpClient()
.AddSingleton<AuthTokenProvider>() // register the AuthTokenProvider
.AddRefitClient<IMoviesApi>(x => new RefitSettings
{

    AuthorizationHeaderValueGetter = async (message, token) => await x.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
}) 
#endregion

.ConfigureHttpClient(x => x.BaseAddress = new Uri("https://localhost:7293")); // register the IMoviesApi

var provider = services.BuildServiceProvider();
var movieApi = provider.GetRequiredService<IMoviesApi>(); // get the IMoviesApi


#region GetAll Movies
// get all movies |  needs authentication
var movies = await movieApi.GetAllMoviesAsync(new GetAllMoviesRequest
{
    Page = 1,
    PageSize = 2,
    Title = null,
    Year = null,
    SortBy = null

});


foreach (var movieObj in movies.Items)
{

    Console.WriteLine(JsonSerializer.Serialize(movies));
}
Console.WriteLine(movies.Page);
Console.WriteLine(movies.HasNextPage);
Console.WriteLine(movies.Total);
#endregion


#region Other Endpoints

var newMovie = await movieApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action" }
});

await movieApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
    Title = "Spiderman 2",
    YearOfRelease = 2002,
    Genres = new[] { "Action", "Adventure" }
});

await movieApi.DeleteMovieAsync(newMovie.Id);




#endregion






Console.ReadKey();
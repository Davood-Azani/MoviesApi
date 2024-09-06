

using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{


    #region InMemory
    //private readonly List<Movie> _movies = new ();
    //public Task<bool> CreateAsync(Movie movie)
    //{
    //    _movies.Add(movie);
    //    return Task.FromResult(true);
    //}

    //public Task<Movie?> GetByIdAsync(Guid id)
    //{
    //    var res = _movies.SingleOrDefault(x => x.Id == id);
    //    return Task.FromResult(res);
    //}

    //public Task<Movie?> GetBySlugAsync(string slug)
    //{
    //    var res = _movies.SingleOrDefault(x => x.Slug == slug);
    //    return Task.FromResult(res);
    //}

    //public Task<IEnumerable<Movie>> GetAllAsync()
    //{


    //    return Task.FromResult(_movies.AsEnumerable());
    //}

    //public Task<bool> UpdateAsync(Movie movie)
    //{
    //    var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
    //    if (movieIndex == -1)
    //    {
    //        return Task.FromResult(false);
    //    }
    //    _movies[movieIndex] = movie; // lists works as arrays

    //    return Task.FromResult(true);
    //}

    //public Task<bool> DeleteByIdAsync(Guid id)
    //{
    //   var removedCount = _movies.RemoveAll(x => x.Id == id);
    //    return Task.FromResult(removedCount > 0);
    //}

    #endregion

    #region Postgres

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();
        // we need this as we are doing multiple operations and insert into multiple tables : Movies and Genres

        var result = await connection.ExecuteAsync(new CommandDefinition(

            """
           insert into movies (id,title,slug,yearofrelease)
           values (@Id,@Title,@Slug,@YearOfRelease)
           """, movie, cancellationToken: token


            ));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                       """
                          insert into genres (movieId,name)
                          values (@MovieId,@Name)
                          """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token
                  ));

            }
        }
        transaction.Commit(); // means transaction is successful
        return result > 0;

    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(
            """
             select m.*, round(avg(r.rating),1) as rating, myr.rating as userrating 
             from movies m
             left join ratings r on m.id = r.movieid
             left join ratings myr on m.id =myr.movieid
             and myr.userid = @userId
             where id = @id
             group by id,userrating
            """, new { id, userId }, cancellationToken: token
            ));

        // as rating and as userrating make it easy to map with dapper with the relatedProps in the movie class, so we don't need to do it manually
        // UserRating and Rating

        //  return movie;  if we don't need to returns back genre so this is enough
        // but we need to return genres as well so we need to get genres as well



        if (movie is null) return null;


        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres where movieid = @MovieId
            """, new { MovieId = id }, cancellationToken: token
            ));


        // movie.Genres = genres.ToList(); // we cant do this because genres is init and we can't assign with = , so we need to add


        foreach (var genre in genres)
        {

            movie.Genres.Add(genre);
        }

        return movie;



    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(new CommandDefinition(
            """
            select m.*, round(avg(r.rating),1) as rating, myr.rating as userrating 
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id =myr.movieid
            and myr.userid = @userId
            where slug = @slug
            group by id,userrating
            """, new { slug, userId }, cancellationToken: token
        ));




        if (movie is null) return null;


        var genres = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres where movieid = @MovieId
            """, new { MovieId = movie.Id }, cancellationToken: token
        ));


        foreach (var genre in genres)
        {

            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                           , m.{options.SortField}
                           order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
                           """;
        }

        var result = await connection.QueryAsync(new CommandDefinition($"""
                                                                        select m.*, 
                                                                               string_agg(distinct g.name, ',') as genres , 
                                                                               round(avg(r.rating), 1) as rating, 
                                                                               myr.rating as userrating
                                                                        from movies m 
                                                                        left join genres g on m.id = g.movieid
                                                                        left join ratings r on m.id = r.movieid
                                                                        left join ratings myr on m.id = myr.movieid
                                                                            and myr.userid = @userId
                                                                        where (@title is null or m.title like ('%' || @title || '%'))
                                                                        and  (@yearofrelease is null or m.yearofrelease = @yearofrelease)
                                                                        group by id, userrating {orderClause}
                                                                        limit @pageSize
                                                                                   offset @pageOffset
                                                                        """, new
        {
            userId = options.UserId,
            title = options.Title,
            yearofrelease = options.YearOfRelease,
            pageSize = options.PageSize,
            pageOffset = (options.Page - 1) * options.PageSize
        }, cancellationToken: token));

        return result.Select(x => new Movie
        {

            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            // Genres =x.genres.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(',')),





        });

    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        using var transaction = connection.BeginTransaction();

        // first we need to delete all genres for this movie
        await connection.ExecuteAsync(new CommandDefinition(
            """
            delete from genres where movieid = @Id
            """, new { movie.Id }, cancellationToken: token

        ));

        foreach (var genre in movie.Genres)
        {

            await connection.ExecuteAsync(new CommandDefinition(
                """
                insert into genres (movieid,name)
                values (@MovieId,@Name)
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token
            ));
        }
        var result = await connection.ExecuteAsync(new CommandDefinition(
             """
            update movies
            set title = @Title,
            slug = @Slug,
            yearofrelease = @YearOfRelease
            where id = @Id
            """, movie, cancellationToken: token
         ));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token)
    {
        var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition(
            """
            delete from genres where movieid = @Id
            """, new { Id = id }, cancellationToken: token

        ));


        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            delete from movies where id = @Id
            """, new { Id = id }, cancellationToken: token
        ));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistByIdAsync(Guid id, CancellationToken token)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        var result = await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            """
            select count(1) from movies where id = @Id
            """, new { Id = id }, cancellationToken: token
        ));

        return result;

    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);
        return await connection.QuerySingleAsync<int>(new CommandDefinition("""
                                                                            select count(id) from movies
                                                                            where (@title is null or title like ('%' || @title || '%'))
                                                                            and  (@yearOfRelease is null or yearofrelease = @yearOfRelease)
                                                                            """, new
        {
            title,
            yearOfRelease
        }, cancellationToken: token));
    }

    #endregion
}
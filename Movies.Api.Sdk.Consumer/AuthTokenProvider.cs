using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace Movies.Api.Sdk.Consumer;

public class AuthTokenProvider(HttpClient client)
{
    private string _cachedToke = string.Empty;
    private static readonly SemaphoreSlim Lock = new(1, 1); // means that only one thread can access the code at a time
    

    public async Task<string> GetTokenAsync()
    {
        try
        {
            // check the time of the cacheToken before using
            if (!string.IsNullOrEmpty(_cachedToke))
            {
                // ReadJwtToken vs ReadToken
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToke); // read the token

                var expiryTimeNext = jwt.Claims.Single(c => c.Type == "exp").Value; // get the expiry time

                var expiryTime =
                    UnixTimeStampToDateTime(int.Parse(expiryTimeNext)); // convert the expiry time to a date time

                if (expiryTime > DateTime.UtcNow) // check if the expiry time is greater than the current time
                {
                    return _cachedToke;
                }
            }

            await Lock.WaitAsync(); // wait for the lock to be released

            var response = await client.PostAsJsonAsync("https://localhost:5003/token", new
            {
                userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
                email = "admin@test.com",
                customCliams = new Dictionary<string, object>
                {
                    { "admin", true },
                    { "trusted_member", true }
                }
            });
            // response.EnsureSuccessStatusCode(); // ensure that the response is successful
            var token = await response.Content.ReadAsStringAsync(); // read the token from the response
            _cachedToke = token;
            return token;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to get token", e);
        }
        finally
        {
            Console.WriteLine(_cachedToke);
            Lock.Release();
        }
    }

    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); // the start of the unix time
        dateTime = dateTime.AddSeconds(unixTimeStamp); // add the seconds to the start of the unix time
        return dateTime;
    }
}
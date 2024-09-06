using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth;

public class ApiKeyAuthFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //var user = context.HttpContext.User;
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, 
                out var extractedApiKey))
        //means  if the api key is not found or if it was not passed in the header

        {
            context.Result = new UnauthorizedObjectResult("Api Key missing");
            return;
        }

        //var apiKey = configuration.GetValue<string>("ApiKey");
        var apiKey = configuration["ApiKey"];
        if (apiKey != extractedApiKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid Api Key");
            
        }

    }
}


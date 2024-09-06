using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Auth;


public class AdminAuthRequirement(string apiKey) : IAuthorizationHandler, IAuthorizationRequirement
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (!context.User.HasClaim(AuthConstants.AdminUserClaimName, AuthConstants.True))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        //  var httpContext = context.Resource as HttpContext;
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
        #region Comment
        // 
        //if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
        //    out var extractedApiKey))
        //{
        //    context.Fail();
        //    return Task.CompletedTask;
        //}


        //if (apiKey != extractedApiKey)
        //{
        //    context.Fail();
        //    return Task.CompletedTask;
        //} 
        #endregion

        // we use this if we want to authorize the user to access the admin panel by Or 
        // as if user get authenticated by api key we need the admin id to be added to the claims
        // of course if we need the admin id, otherwise we can just use the api key
        //var identity = (ClaimsIdentity)httpContext.User.Identity!;
        //identity.AddClaim(new Claim("userid", "ex admin id"));




        context.Succeed(this);
        return Task.CompletedTask;

    }
}

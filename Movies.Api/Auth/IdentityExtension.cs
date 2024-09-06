using System.Security.Claims;

namespace Movies.Api.Auth
{
    public static class IdentityExtension
    {
        public static Guid? GetUserId(this HttpContext context)
        {
            //var userid = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // NameIdentifier is id of the user
            // but we want to get the userid which is a guid
            //var userid = context.User.FindFirst(x=>x.Type=="userid")?.Value;
            var userid = context.User.Claims.FirstOrDefault(x => x.Type == "userid");


            if(Guid.TryParse(userid?.Value ,out var parsedId))
            {
                return parsedId;
            }

            return null;


        }

    }
}

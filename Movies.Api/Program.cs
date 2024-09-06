using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);


var config = builder.Configuration;

builder.Services.AddAuthentication(c =>
{

    //3Ds!  

    c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;



}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
        ValidateIssuerSigningKey = true, // means that the token is signed and the server can validate it
        ValidateLifetime = true, // means that the token is not expired
        ValidateIssuer = true, // means that the token was issued by a trusted party
        ValidateAudience = true, // means that the token is addressed to this server
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],

    };
});

//builder.Services.AddAuthorization(x =>
//{
//    x.AddPolicy("JustAdmin" , p=>p.RequireClaim("admin" , "true"));
//});


builder.Services.AddAuthorizationBuilder()
    //.AddPolicy(AuthConstants.AdminUserPolicyName,
    //    p =>
    //        p.RequireClaim(AuthConstants.AdminUserClaimName, AuthConstants.True))

    .AddPolicy(AuthConstants.AdminUserPolicyName, p =>
        p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)))


    .AddPolicy(AuthConstants.TrustedMemberPolicyName, p =>
        p.RequireAssertion(c =>

            c.User.HasClaim(AuthConstants.TrustedMemberClaimName, AuthConstants.True) ||
            c.User.HasClaim(AuthConstants.AdminUserClaimName, AuthConstants.True)

        ));




builder.Services.AddApiVersioning(x =>
    {


        x.DefaultApiVersion =
            new ApiVersion(1.0); // means that if the version is not specified, the default version is 1.0
        x.AssumeDefaultVersionWhenUnspecified =
            true; // means that if the version is not specified, the default version is 1.0
        x.ReportApiVersions = true; // means that the version is reported in the response header
        x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");


    }).AddMvc() // adding service of versioning
    .AddApiExplorer()
    ;

//builder.Services.AddResponseCaching();
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddOutputCache(x =>
{

    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c =>
    {
        c.Cache()
            .Expire(TimeSpan.FromMinutes(1)) // means that the cache will expire after 1 minute
            .SetVaryByQuery(new[] { "title", "year", "pageSize", "page" }) // means that the cache will vary by the query parameters and these
                                                                           // are the query parameters and 
                                                                           //the cache will be different for each query parameter
                                                                           // with help of this we can cache the response of the request with
                                                                           // different query parameters
            .Tag("movies"); // means that the cache will be tagged with movies and we can invalidate the cache with this tag


    });

});



builder.Services.AddControllers();


// add health check
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);





//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {

        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }





    });
}




// add health check

app.MapHealthChecks("_health");







app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors(); // this must be always before caching

//app.UseResponseCaching();
app.UseOutputCache();// this only cache 200 cache and and the other response will not be cached
// then only get and head request will be cached and the other request will not be cached like post and put and delete
// responses with set cookie will not be cached and also response to authenticated users will not be cached


app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();


// Initialize the database
var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();


app.Run();
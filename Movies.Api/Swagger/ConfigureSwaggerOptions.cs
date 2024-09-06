using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IHostEnvironment environment)
    : IConfigureOptions<SwaggerGenOptions>
{


    public void Configure(SwaggerGenOptions options)
    {


        // add swagger doc for each api version

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = environment.ApplicationName,
                Version = description.ApiVersion.ToString(),
                Description = "A simple example ASP.NET Core Web API",
                Contact = new OpenApiContact
                {
                    Name = "OverLoad",
                    Email = "f00000b@gmail.com"

                }
            });
        }

        // add security definition to swagger
        #region security
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header, // add token to header
            Description = "Please provide a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http, // use http
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }); 
        #endregion
    }
}


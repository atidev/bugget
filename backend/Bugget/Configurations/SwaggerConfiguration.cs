using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Bugget.Entities.Options;

namespace Bugget.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Report API", Version = "v1" });
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            var authHeaders = configuration.GetSection("ExternalSettings:Authentication").Get<AuthHeadersOptions>();
            if (authHeaders != null)
            {
                if (!string.IsNullOrEmpty(authHeaders.UserIdHeaderName))
                {
                    c.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Name = authHeaders.UserIdHeaderName,
                        Description = "User ID header"
                    });
                }

                if (!string.IsNullOrEmpty(authHeaders.TeamIdHeaderName))
                {
                    c.AddSecurityDefinition("TeamId", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Name = authHeaders.TeamIdHeaderName,
                        Description = "Team ID header"
                    });
                }

                if (!string.IsNullOrEmpty(authHeaders.OrganizationIdHeaderName))
                {
                    c.AddSecurityDefinition("OrganizationId", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Name = authHeaders.OrganizationIdHeaderName,
                        Description = "Organization ID header"
                    });
                }

                var securityRequirements = new List<OpenApiSecurityRequirement>();
                
                if (!string.IsNullOrEmpty(authHeaders.UserIdHeaderName))
                {
                    securityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "UserId" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                if (!string.IsNullOrEmpty(authHeaders.TeamIdHeaderName))
                {
                    securityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "TeamId" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                if (!string.IsNullOrEmpty(authHeaders.OrganizationIdHeaderName))
                {
                    securityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OrganizationId" }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                foreach (var requirement in securityRequirements)
                {
                    c.AddSecurityRequirement(requirement);
                }
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger(c => c.RouteTemplate = "_internal/swagger/{documentName}/swagger.json");
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/_internal/swagger/v1/swagger.json", "Report API v1");
            c.RoutePrefix = "_internal/swagger";
        });

        return app;
    }
} 
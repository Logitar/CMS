using Logitar.Cms.Contracts.Constants;
using Microsoft.OpenApi.Models;

namespace Logitar.Cms.Web.Extensions;

public static class OpenApiExtensions
{
  private const string Title = "CMS API";

  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.SwaggerDoc(name: $"v{Application.Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Logitar Team",
          Url = new Uri("https://github.com/Logitar/CMS", UriKind.Absolute)
        },
        Description = "Content management system.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Logitar/CMS/blob/main/LICENSE", UriKind.Absolute)
        },
        Title = Title,
        Version = $"v{Application.Version}"
      });
    });

    return services;
  }

  public static void UseOpenApi(this IApplicationBuilder builder)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{Application.Version.Major}/swagger.json",
      name: $"{Title} v{Application.Version}"
    ));
  }
}

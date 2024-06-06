using Logitar.Cms.Constants;
using Microsoft.OpenApi.Models;

namespace Logitar.Cms.Extensions;

internal static class OpenApiExtensions
{
  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.SwaggerDoc(name: $"v{Api.Version.Major}", new OpenApiInfo
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
        Title = Api.Title,
        Version = $"v{Api.Version}"
      });
    });

    return services;
  }

  public static void UseOpenApi(this IApplicationBuilder builder)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{Api.Version.Major}/swagger.json",
      name: $"{Api.Title} v{Api.Version}"
    ));
  }
}

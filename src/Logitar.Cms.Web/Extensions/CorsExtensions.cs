using Logitar.Cms.Web.Settings;

namespace Logitar.Cms.Web.Extensions;

public static class CorsExtensions
{
  public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
  {
    CorsSettings settings = configuration.GetSection("Cors").Get<CorsSettings>() ?? new();

    return services.AddCors(settings);
  }

  public static IServiceCollection AddCors(this IServiceCollection services, CorsSettings settings)
  {
    return services
      .AddCors(options => options.AddDefaultPolicy(policy =>
      {
        if (settings.AllowedOrigins?.Contains("*") == true)
        {
          policy.AllowAnyOrigin();
        }
        else if (settings.AllowedOrigins?.Any() == true)
        {
          policy.WithOrigins(settings.AllowedOrigins);
        }

        if (settings.AllowCredentials)
        {
          policy.AllowCredentials();
        }
        else
        {
          policy.DisallowCredentials();
        }

        if (settings.AllowedMethods?.Contains("*") == true)
        {
          policy.AllowAnyMethod();
        }
        else if (settings.AllowedMethods?.Any() == true)
        {
          policy.WithMethods(settings.AllowedMethods);
        }

        if (settings.AllowedHeaders?.Contains("*") == true)
        {
          policy.AllowAnyHeader();
        }
        else if (settings.AllowedHeaders?.Any() == true)
        {
          policy.WithHeaders(settings.AllowedHeaders);
        }
      }));
  }
}

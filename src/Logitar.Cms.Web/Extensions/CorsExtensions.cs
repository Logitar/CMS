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
        else if (settings.AllowedOrigins != null)
        {
          policy.WithOrigins(settings.AllowedOrigins);
        }

        if (settings.AllowedMethods?.Contains("*") == true)
        {
          policy.AllowAnyMethod();
        }
        else if (settings.AllowedMethods != null)
        {
          policy.WithMethods(settings.AllowedMethods);
        }

        if (settings.AllowedHeaders?.Contains("*") == true)
        {
          policy.AllowAnyHeader();
        }
        else if (settings.AllowedHeaders != null)
        {
          policy.WithHeaders(settings.AllowedHeaders);
        }
      }));
  }
}

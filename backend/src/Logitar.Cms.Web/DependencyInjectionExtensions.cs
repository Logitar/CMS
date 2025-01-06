using Logitar.Cms.Core;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Settings;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddControllersWithViews()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    CookiesSettings cookiesSettings = configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);

    CorsSettings corsSettings = configuration.GetSection(CorsSettings.SectionKey).Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);

    OpenAuthenticationSettings openAuthenticationSettings = configuration.GetSection(OpenAuthenticationSettings.SectionKey).Get<OpenAuthenticationSettings>() ?? new();
    services.AddSingleton(openAuthenticationSettings);

    return services
      .AddSingleton<IApplicationContext, HttpApplicationContext>()
      .AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();
  }
}

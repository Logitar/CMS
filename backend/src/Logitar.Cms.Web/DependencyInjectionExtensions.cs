using Logitar.Cms.Core;
using Logitar.Cms.Web.Filters;
using Logitar.Cms.Web.Settings;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services, IConfiguration configuration)
  {
    services
      .AddControllersWithViews(options =>
      {
        options.Filters.Add<ExceptionHandling>();
        options.Filters.Add<LoggingFilter>();
      })
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    CookiesSettings cookiesSettings = configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    return services
      .AddLogitarCmsCore()
      .AddSingleton<IActivityContextResolver, HttpActivityContextResolver>();
  }
}

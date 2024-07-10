using Logitar.Cms.Core;
using Logitar.Cms.Web.Filters;
using Logitar.Cms.Web.Settings;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
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

    return services
      .AddLogitarCmsCore()
      .AddSingleton(GetCookiesSettings)
      .AddSingleton<IActivityContextResolver, HttpActivityContextResolver>();
  }

  public static CookiesSettings GetCookiesSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CookiesSettings.SectionKey).Get<CookiesSettings>() ?? new();
  }
}

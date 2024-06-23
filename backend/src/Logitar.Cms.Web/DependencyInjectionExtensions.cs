using Logitar.Cms.Core;
using Logitar.Cms.Web.Authentication;
using Logitar.Cms.Web.Filters;
using Logitar.Cms.Web.Settings;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<ExceptionHandling>()) // TODO(fpion): LoggingFilter
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    return services
      .AddLogitarCmsCore()
      .AddSingleton(InitializeOAuthSettings)
      .AddTransient<IActivityContextResolver, HttpActivityContextResolver>()
      .AddTransient<IOpenAuthenticationService, OpenAuthenticationService>();
  }

  private static OpenAuthenticationSettings InitializeOAuthSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("OAuth").Get<OpenAuthenticationSettings>() ?? new();
  }
}

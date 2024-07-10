using Logitar.Cms.Core;
using Logitar.Cms.Web.Filters;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services
      .AddControllersWithViews(options =>
      {
        options.Filters.Add<ExceptionHandling>();
        // TODO(fpion): LoggingFilter
      })
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    return services.AddLogitarCmsCore();
  }
}

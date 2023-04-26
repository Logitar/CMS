using Logitar.Cms.Core;
using Logitar.Cms.Web.Filters;
using System.Text.Json.Serialization;

namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(options => options.Filters.Add<ExceptionHandlingFilter>())
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    services.AddLogitarCmsCore();

    services.AddDistributedMemoryCache();

    return services;
  }
}

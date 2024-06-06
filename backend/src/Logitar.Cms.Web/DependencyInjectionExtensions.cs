namespace Logitar.Cms.Web;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsWeb(this IServiceCollection services)
  {
    services.AddControllersWithViews(/*options => options.Filters.Add<ExceptionHandling>()*/) // TODO(fpion): implement
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    return services/*.AddLogitarCmsCore()*/; // TODO(fpion): implement
  }
}

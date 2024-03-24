using Logitar.Identity.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsCore(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}

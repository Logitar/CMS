using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Resources;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Resources;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Cms.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsCore(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddFacades()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IResourceService, ResourceService>();
  }
}

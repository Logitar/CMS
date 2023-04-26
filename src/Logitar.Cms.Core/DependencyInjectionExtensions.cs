using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Resources;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Mapping;
using Logitar.Cms.Core.Resources;
using Logitar.Cms.Core.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Logitar.Cms.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsCore(this IServiceCollection services)
  {
    Assembly assembly = typeof(DependencyInjectionExtensions).Assembly;

    return services
      .AddAutoMapper(assembly)
      .AddFacades()
      .AddMediatR(config => config.RegisterServicesFromAssembly(assembly))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IMappingService, MappingService>()
      .AddTransient<IRequestPipeline, RequestPipeline>();
  }

  private static IServiceCollection AddFacades(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IResourceService, ResourceService>()
      .AddTransient<ISessionService, SessionService>();
  }
}

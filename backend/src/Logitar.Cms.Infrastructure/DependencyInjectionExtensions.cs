using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Infrastructure.Caching;
using Logitar.Cms.Infrastructure.Converters;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarCmsCore()
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(services => new EventSerializer(services.GetLogitarCmsJsonConverters()));
  }

  public static IReadOnlyCollection<JsonConverter> GetLogitarCmsJsonConverters(this IServiceProvider serviceProvider)
  {
    List<JsonConverter> converters = [];
    converters.AddRange(serviceProvider.GetLogitarIdentityJsonConverters());

    converters.Add(new ConfigurationIdConverter());
    converters.Add(new JwtSecretConverter());
    converters.Add(new LanguageIdConverter());

    return converters.AsReadOnly();
  }
}

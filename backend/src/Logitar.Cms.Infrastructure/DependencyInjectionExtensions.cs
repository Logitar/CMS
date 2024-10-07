using Logitar.Cms.Core;
using Logitar.Cms.Core.Caching;
using Logitar.Cms.Infrastructure.Caching;
using Logitar.Cms.Infrastructure.Converters;
using Logitar.Cms.Infrastructure.Settings;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
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
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(InitializeEventSerializer);
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CachingSettings.SectionKey).Get<CachingSettings>() ?? new();
  }

  private static EventSerializer InitializeEventSerializer(IServiceProvider serviceProvider) => new(serviceProvider.GetJsonConverters());
  public static IEnumerable<JsonConverter> GetJsonConverters(this IServiceProvider serviceProvider)
  {
    return serviceProvider.GetLogitarIdentityJsonConverters().Concat(
    [
      new LanguageIdConverter(),
      new LocaleConverter()
    ]);
  }
}

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
      .AddTransient<IEventBus, CmsEventBus>()
      .AddSingleton<IEventSerializer>(serviceProvider => new EventSerializer(serviceProvider.GetLogitarCmsJsonConverters()));
  }

  private static IReadOnlyCollection<JsonConverter> GetLogitarCmsJsonConverters(this IServiceProvider serviceProvider)
  {
    IEnumerable<JsonConverter> identityConverters = serviceProvider.GetLogitarIdentityJsonConverters();

    List<JsonConverter> converters =
    [
      .. identityConverters,
      new ConfigurationIdConverter(),
      new ContentIdConverter(),
      new ContentTypeIdConverter(),
      new FieldTypeIdConverter(),
      new IdentifierConverter(),
      new JwtSecretConverter(),
      new LanguageIdConverter(),
      new PlaceholderConverter()
    ];

    return converters.AsReadOnly();
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }
}

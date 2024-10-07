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
      .AddSingleton(GetCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(services => new EventSerializer(services.GetLogitarCmsJsonConverters()))
      .AddTransient<IEventBus, CmsEventBus>();
  }

  public static CachingSettings GetCachingSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection(CachingSettings.SectionKey).Get<CachingSettings>() ?? new();
  }

  public static IReadOnlyCollection<JsonConverter> GetLogitarCmsJsonConverters(this IServiceProvider serviceProvider)
  {
    List<JsonConverter> converters = [];
    converters.AddRange(serviceProvider.GetLogitarIdentityJsonConverters());

    converters.Add(new ConfigurationIdConverter());
    converters.Add(new ContentIdConverter());
    converters.Add(new ContentTypeIdConverter());
    converters.Add(new FieldTypeIdConverter());
    converters.Add(new IdentifierConverter());
    converters.Add(new JwtSecretConverter());
    converters.Add(new LanguageIdConverter());
    converters.Add(new PlaceholderConverter());

    return converters.AsReadOnly();
  }
}

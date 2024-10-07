using Logitar.Cms.Core;
using Logitar.Cms.Infrastructure.Converters;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPriceWatchInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarCmsCore()
      .AddSingleton<IEventSerializer>(InitializeEventSerializer)
      .AddTransient<IEventBus, EventBus>();
  }

  private static EventSerializer InitializeEventSerializer(IServiceProvider serviceProvider) => new(serviceProvider.GetJsonConverters());
  public static IEnumerable<JsonConverter> GetJsonConverters(this IServiceProvider _) =>
  [
    new LanguageIdConverter(),
    new LocaleConverter()
  ];
}

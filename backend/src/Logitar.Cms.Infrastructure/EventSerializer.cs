using Logitar.Cms.Infrastructure.Converters;

namespace Logitar.Cms.Infrastructure;

internal class EventSerializer : EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new LanguageIdConverter());
    SerializerOptions.Converters.Add(new LocaleConverter());
  }
}

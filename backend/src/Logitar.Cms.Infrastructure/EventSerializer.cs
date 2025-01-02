using Logitar.Cms.Infrastructure.Converters;
using Logitar.Identity.Infrastructure.Converters;

namespace Logitar.Cms.Infrastructure;

public class EventSerializer : Identity.Infrastructure.EventSerializer
{
  public EventSerializer(PasswordConverter passwordConverter) : base(passwordConverter)
  {
  }

  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new FieldTypeIdConverter());
    SerializerOptions.Converters.Add(new LanguageIdConverter());
  }
}

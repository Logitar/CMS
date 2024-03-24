using Logitar.Cms.Core.Configurations;

namespace Logitar.Cms.Infrastructure.Converters;

public class ConfigurationIdConverter : JsonConverter<ConfigurationId>
{
  public override ConfigurationId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return new ConfigurationId();
  }

  public override void Write(Utf8JsonWriter writer, ConfigurationId configurationId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(configurationId.Value);
  }
}

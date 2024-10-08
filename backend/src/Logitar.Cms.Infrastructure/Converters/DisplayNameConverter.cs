using Logitar.Cms.Core;

namespace Logitar.Cms.Infrastructure.Converters;

internal class DisplayNameConverter : JsonConverter<DisplayName>
{
  public override DisplayName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return DisplayName.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, DisplayName displayname, JsonSerializerOptions options)
  {
    writer.WriteStringValue(displayname.Value);
  }
}

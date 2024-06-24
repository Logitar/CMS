using Logitar.Cms.Core.ContentTypes;

namespace Logitar.Cms.Infrastructure.Converters;

public class PlaceholderConverter : JsonConverter<PlaceholderUnit>
{
  public override PlaceholderUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return PlaceholderUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, PlaceholderUnit placeholder, JsonSerializerOptions options)
  {
    writer.WriteStringValue(placeholder.Value);
  }
}

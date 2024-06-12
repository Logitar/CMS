using Logitar.Cms.Core.ContentTypes;

namespace Logitar.Cms.Infrastructure.Converters;

public class ContentTypeIdConverter : JsonConverter<ContentTypeId?>
{
  public override ContentTypeId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return ContentTypeId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, ContentTypeId? fieldTypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(fieldTypeId?.Value);
  }
}

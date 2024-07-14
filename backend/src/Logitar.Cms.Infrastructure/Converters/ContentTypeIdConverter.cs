using Logitar.Cms.Core.ContentTypes;

namespace Logitar.Cms.Infrastructure.Converters;

public class ContentTypeIdConverter : JsonConverter<ContentTypeId>
{
  public override ContentTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? new ContentTypeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ContentTypeId contentTypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(contentTypeId.Value);
  }
}

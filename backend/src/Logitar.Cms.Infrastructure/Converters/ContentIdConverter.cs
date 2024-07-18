using Logitar.Cms.Core.Contents;

namespace Logitar.Cms.Infrastructure.Converters;

public class ContentIdConverter : JsonConverter<ContentId>
{
  public override ContentId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return value == null ? new ContentId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ContentId contentId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(contentId.Value);
  }
}

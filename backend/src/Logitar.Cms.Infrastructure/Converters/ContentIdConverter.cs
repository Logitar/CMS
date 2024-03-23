using Logitar.Cms.Core.Contents;

namespace Logitar.Cms.Infrastructure.Converters;

public class ContentIdConverter : JsonConverter<ContentId>
{
  public override ContentId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return ContentId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, ContentId contentType, JsonSerializerOptions options)
  {
    writer.WriteStringValue(contentType.Value);
  }
}

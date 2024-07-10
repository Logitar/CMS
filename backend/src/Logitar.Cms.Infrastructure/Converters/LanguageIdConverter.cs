using Logitar.Cms.Core.Languages;

namespace Logitar.Cms.Infrastructure.Converters;

public class LanguageIdConverter : JsonConverter<LanguageId>
{
  public override LanguageId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return LanguageId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, LanguageId languageId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(languageId.Value);
  }
}

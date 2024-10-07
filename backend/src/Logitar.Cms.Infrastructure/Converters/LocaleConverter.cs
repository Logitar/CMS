using Logitar.Cms.Core.Languages;

namespace Logitar.Cms.Infrastructure.Converters;

internal class LocaleConverter : JsonConverter<Locale>
{
  public override Locale? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? code = reader.GetString();
    return string.IsNullOrWhiteSpace(code) ? null : new(code);
  }

  public override void Write(Utf8JsonWriter writer, Locale locale, JsonSerializerOptions options)
  {
    writer.WriteStringValue(locale.Code);
  }
}

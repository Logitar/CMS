using Logitar.Cms.Core;
using Logitar.Cms.Core.Configurations;

namespace Logitar.Cms.Infrastructure.Converters;

internal class UniqueNameConverter : JsonConverter<UniqueName>
{
  private readonly UniqueNameSettings _uniqueNameSettings = new(allowedCharacters: null);

  public override UniqueName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new(_uniqueNameSettings, value);
  }

  public override void Write(Utf8JsonWriter writer, UniqueName uniquename, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniquename.Value);
  }
}

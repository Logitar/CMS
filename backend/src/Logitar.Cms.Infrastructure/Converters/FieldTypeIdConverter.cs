using Logitar.Cms.Core.FieldTypes;

namespace Logitar.Cms.Infrastructure.Converters;

internal class FieldTypeIdConverter : JsonConverter<FieldTypeId>
{
  public override FieldTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, FieldTypeId fieldtypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(fieldtypeId.Value);
  }
}

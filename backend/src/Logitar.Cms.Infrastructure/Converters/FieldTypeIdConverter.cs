using Logitar.Cms.Core.FieldTypes;

namespace Logitar.Cms.Infrastructure.Converters;

public class FieldTypeIdConverter : JsonConverter<FieldTypeId>
{
  public override FieldTypeId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return FieldTypeId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, FieldTypeId fieldTypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(fieldTypeId.Value);
  }
}

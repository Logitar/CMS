using Logitar.Cms.Core;

namespace Logitar.Cms.Infrastructure.Converters;

public class IdentifierConverter : JsonConverter<IdentifierUnit>
{
  public override IdentifierUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return IdentifierUnit.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, IdentifierUnit identifier, JsonSerializerOptions options)
  {
    writer.WriteStringValue(identifier.Value);
  }
}

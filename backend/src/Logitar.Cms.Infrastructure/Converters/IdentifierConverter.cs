using Logitar.Cms.Core;

namespace Logitar.Cms.Infrastructure.Converters;

internal class IdentifierConverter : JsonConverter<Identifier>
{
  public override Identifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Identifier.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Identifier identifier, JsonSerializerOptions options)
  {
    writer.WriteStringValue(identifier.Value);
  }
}

using Logitar.Cms.Core.Archetypes;

namespace Logitar.Cms.Infrastructure.Converters;

public class ArchetypeIdConverter : JsonConverter<ArchetypeId>
{
  public override ArchetypeId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return ArchetypeId.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, ArchetypeId archetypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(archetypeId.Value);
  }
}

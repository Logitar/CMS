using Logitar.Cms.Core.Configurations;

namespace Logitar.Cms.Infrastructure.Converters;

public class JwtSecretConverter : JsonConverter<JwtSecretUnit>
{
  public override JwtSecretUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return JwtSecretUnit.CreateOrGenerate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, JwtSecretUnit jwtSecret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(jwtSecret.Value);
  }
}

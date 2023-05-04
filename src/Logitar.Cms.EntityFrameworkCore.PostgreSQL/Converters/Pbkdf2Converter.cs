using Logitar.Cms.Core.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Converters;

internal class Pbkdf2Converter : JsonConverter<Pbkdf2>
{
  public override Pbkdf2? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? s = reader.GetString();

    return s == null ? null : Pbkdf2.Parse(s);
  }

  public override void Write(Utf8JsonWriter writer, Pbkdf2 value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(value.ToString());
  }
}

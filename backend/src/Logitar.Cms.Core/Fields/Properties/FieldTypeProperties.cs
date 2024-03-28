using Logitar.Cms.Contracts.Fields;
using System.Text.Json.Serialization;

namespace Logitar.Cms.Core.Fields.Properties;

public abstract record class FieldTypeProperties
{
  [JsonIgnore]
  public abstract DataType DataType { get; }
}

using Logitar.Cms.Contracts.Fields;

namespace Logitar.Cms.Core.Fields.Properties;

public abstract record FieldTypeProperties
{
  [JsonIgnore]
  public abstract DataType DataType { get; }
}

using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public abstract record FieldTypeProperties
{
  public abstract DataType DataType { get; }
}

using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public abstract record BaseProperties
{
  public abstract DataType DataType { get; }
}

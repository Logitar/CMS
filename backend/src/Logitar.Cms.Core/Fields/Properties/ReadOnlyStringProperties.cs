using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

public record ReadOnlyStringProperties : FieldTypeProperties, IStringProperties
{
  public override DataType DataType => DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  public ReadOnlyStringProperties() : this(null, null, null)
  {
  }

  public ReadOnlyStringProperties(IStringProperties @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }

  public ReadOnlyStringProperties(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringPropertiesValidator().ValidateAndThrow(this);
  }
}

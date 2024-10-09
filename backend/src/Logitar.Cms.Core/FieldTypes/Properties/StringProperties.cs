using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record StringProperties : BaseProperties, IStringProperties
{
  public override DataType DataType { get; } = DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  public StringProperties(IStringProperties @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }

  public StringProperties(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringPropertiesValidator().ValidateAndThrow(this);
  }
}

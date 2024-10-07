using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record ReadOnlyStringProperties : FieldTypeProperties, IStringProperties
{
  public override DataType DataType => DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  public ReadOnlyStringProperties() : this(null, null, null)
  {
  }

  public ReadOnlyStringProperties(IStringProperties @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern?.CleanTrim())
  {
  }

  [JsonConstructor]
  public ReadOnlyStringProperties(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringPropertiesValidator().ValidateAndThrow(this);
  }
}

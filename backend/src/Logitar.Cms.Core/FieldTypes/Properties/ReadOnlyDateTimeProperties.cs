using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Contracts.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;

namespace Logitar.Cms.Core.FieldTypes.Properties;

public record ReadOnlyDateTimeProperties : FieldTypeProperties, IDateTimeProperties
{
  public override DataType DataType => DataType.DateTime;

  public DateTime? MinimumValue { get; }
  public DateTime? MaximumValue { get; }

  public ReadOnlyDateTimeProperties() : this(null, null)
  {
  }

  public ReadOnlyDateTimeProperties(IDateTimeProperties dateTime) : this(dateTime.MinimumValue, dateTime.MaximumValue)
  {
  }

  [JsonConstructor]
  public ReadOnlyDateTimeProperties(DateTime? minimumValue, DateTime? maximumValue)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    new DateTimePropertiesValidator().ValidateAndThrow(this);
  }
}

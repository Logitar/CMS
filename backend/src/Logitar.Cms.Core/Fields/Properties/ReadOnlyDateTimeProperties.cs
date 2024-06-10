using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Contracts.Fields.Properties;

namespace Logitar.Cms.Core.Fields.Properties;

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

  public ReadOnlyDateTimeProperties(DateTime? minimumValue, DateTime? maximumValue)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    new DateTimePropertiesValidator().ValidateAndThrow(this);
  }
}

namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record DateTimeProperties : IDateTimeProperties
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }

  public DateTimeProperties() : this(minimumValue: null, maximumValue: null)
  {
  }

  public DateTimeProperties(IDateTimeProperties dateTime) : this(dateTime.MinimumValue, dateTime.MaximumValue)
  {
  }

  public DateTimeProperties(DateTime? minimumValue, DateTime? maximumValue)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
  }
}

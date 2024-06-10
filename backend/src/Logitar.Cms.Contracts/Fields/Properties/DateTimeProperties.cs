namespace Logitar.Cms.Contracts.Fields.Properties;

public record DateTimeProperties : IDateTimeProperties
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }
}

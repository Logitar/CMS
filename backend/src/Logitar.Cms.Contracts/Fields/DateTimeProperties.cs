namespace Logitar.Cms.Contracts.Fields;

public record DateTimeProperties
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }
}

namespace Logitar.Cms.Contracts.Fields;

public record TextProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
}

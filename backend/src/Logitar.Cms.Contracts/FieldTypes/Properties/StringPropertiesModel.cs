namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record StringPropertiesModel : IStringProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }
}

namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public record TextPropertiesModel : ITextProperties
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
}

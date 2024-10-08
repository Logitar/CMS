namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public interface IStringProperties
{
  int? MinimumLength { get; set; }
  int? MaximumLength { get; set; }
  string? Pattern { get; set; }
}

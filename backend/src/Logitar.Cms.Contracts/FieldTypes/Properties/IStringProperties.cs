namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public interface IStringProperties
{
  int? MinimumLength { get; }
  int? MaximumLength { get; }
  string? Pattern { get; }
}

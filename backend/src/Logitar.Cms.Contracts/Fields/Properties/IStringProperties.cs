namespace Logitar.Cms.Contracts.Fields.Properties;

public interface IStringProperties
{
  int? MinimumLength { get; }
  int? MaximumLength { get; }
  string? Pattern { get; }
}

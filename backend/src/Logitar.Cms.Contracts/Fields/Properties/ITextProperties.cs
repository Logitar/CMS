namespace Logitar.Cms.Contracts.Fields.Properties;

public interface ITextProperties
{
  string ContentType { get; }
  int? MinimumLength { get; }
  int? MaximumLength { get; }
}

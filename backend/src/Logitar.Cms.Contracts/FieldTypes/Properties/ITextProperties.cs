namespace Logitar.Cms.Contracts.FieldTypes.Properties;

public interface ITextProperties
{
  string ContentType { get; }
  int? MinimumLength { get; }
  int? MaximumLength { get; }
}

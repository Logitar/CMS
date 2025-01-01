namespace Logitar.Cms.Core.Fields.Settings;

public interface IRichTextSettings
{
  string ContentType { get; }
  int? MinimumLength { get; }
  int? MaximumLength { get; }
}

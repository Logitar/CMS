using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record RichTextSettings : FieldTypeSettings, IRichTextSettings
{
  public override DataType DataType { get; } = DataType.RichText;

  public string ContentType { get; }
  public int? MinimumLength { get; }
  public int? MaximumLength { get; }

  [JsonConstructor]
  public RichTextSettings(string contentType, int? minimumLength = null, int? maximumLength = null)
  {
    ContentType = contentType;
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    new RichTextSettingsValidator().ValidateAndThrow(this);
  }

  public RichTextSettings(IRichTextSettings richText)
  {
    ContentType = richText.ContentType;
    MinimumLength = richText.MinimumLength;
    MaximumLength = richText.MaximumLength;
    new RichTextSettingsValidator().ValidateAndThrow(this);
  }
}

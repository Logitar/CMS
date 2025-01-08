using FluentValidation;
using Logitar.Cms.Core.Fields.Validators;

namespace Logitar.Cms.Core.Fields.Settings;

public record TagsSettings : FieldTypeSettings, ITagsSettings
{
  [JsonIgnore]
  public override DataType DataType { get; } = DataType.Tags;

  [JsonConstructor]
  public TagsSettings()
  {
    new TagsSettingsValidator().ValidateAndThrow(this);
  }

  public TagsSettings(ITagsSettings _)
  {
    new TagsSettingsValidator().ValidateAndThrow(this);
  }
}

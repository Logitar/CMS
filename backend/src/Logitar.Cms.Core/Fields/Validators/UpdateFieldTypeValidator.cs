using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields.Validators;

internal class UpdateFieldTypeValidator : AbstractValidator<UpdateFieldTypePayload>
{
  public UpdateFieldTypeValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(FieldType.UniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Boolean != null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanSettingsValidator()));
    When(x => x.DateTime != null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimeSettingsValidator()));
    When(x => x.Number != null, () => RuleFor(x => x.Number!).SetValidator(new NumberSettingsValidator()));
    When(x => x.RelatedContent != null, () => RuleFor(x => x.RelatedContent!).SetValidator(new RelatedContentSettingsValidator()));
    When(x => x.RichText != null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextSettingsValidator()));
    When(x => x.Select != null, () => RuleFor(x => x.Select!).SetValidator(new SelectSettingsValidator()));
    When(x => x.String != null, () => RuleFor(x => x.String!).SetValidator(new StringSettingsValidator()));
    When(x => x.Tags != null, () => RuleFor(x => x.Tags!).SetValidator(new TagsSettingsValidator()));
  }
}

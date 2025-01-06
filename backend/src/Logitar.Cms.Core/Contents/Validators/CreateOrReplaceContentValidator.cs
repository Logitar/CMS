using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents.Validators;

internal class CreateOrReplaceContentValidator : AbstractValidator<CreateOrReplaceContentPayload>
{
  public CreateOrReplaceContentValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName(Content.UniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleForEach(x => x.FieldValues).SetValidator(new FieldValueValidator());
  }
}

using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents.Validators;

internal class SaveContentLocaleValidator : AbstractValidator<SaveContentLocalePayload>
{
  public SaveContentLocaleValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName(Content.UniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}

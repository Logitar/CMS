using FluentValidation;
using Logitar.Cms.Contracts.Contents;

namespace Logitar.Cms.Core.Contents.Validators;

internal class CreateOrReplaceContentLocaleValidator : AbstractValidator<CreateOrReplaceContentLocalePayload>
{
  public CreateOrReplaceContentLocaleValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName(Content.UniqueNameSettings);
  }
}

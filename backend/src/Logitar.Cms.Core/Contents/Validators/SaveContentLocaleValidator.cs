using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Contents.Validators;

public class SaveContentLocaleValidator : AbstractValidator<SaveContentLocalePayload>
{
  public SaveContentLocaleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));

    RuleForEach(x => x.Fields).SetValidator(new FieldValueValidator());
  }
}

using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class CreateFieldTypeValidator : AbstractValidator<CreateFieldTypePayload>
{
  public CreateFieldTypeValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    When(x => x.StringProperties != null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()))
      .Otherwise(() => RuleFor(x => x.StringProperties).NotNull());
  }
}

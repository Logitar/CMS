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

    When(x => GetDataType(x) == null, () => RuleFor(x => x).Must(x => GetDataType(x) != null)
      .WithErrorCode(nameof(CreateFieldTypeValidator))
      .WithMessage(x => $"Only one of the following must be provided: {nameof(x.BooleanProperties)}, {nameof(x.StringProperties)}, {nameof(x.TextProperties)}."))
      .Otherwise(() =>
      {
        When(x => x.BooleanProperties != null, () => RuleFor(x => x.BooleanProperties!).SetValidator(new BooleanPropertiesValidator()));
        When(x => x.StringProperties != null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()));
        When(x => x.TextProperties != null, () => RuleFor(x => x.TextProperties!).SetValidator(new TextPropertiesValidator()));
      });
  }

  private static DataType? GetDataType(CreateFieldTypePayload payload)
  {
    List<DataType> dataTypes = new(capacity: 3);
    if (payload.BooleanProperties != null)
    {
      dataTypes.Add(DataType.Boolean);
    }
    if (payload.StringProperties != null)
    {
      dataTypes.Add(DataType.String);
    }
    if (payload.TextProperties != null)
    {
      dataTypes.Add(DataType.Text);
    }
    return dataTypes.Count == 1 ? dataTypes.Single() : null;
  }
}

using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes.Validators;

internal class CreateOrReplaceFieldTypeValidator : AbstractValidator<CreateOrReplaceFieldTypePayload>
{
  public CreateOrReplaceFieldTypeValidator(DataType? dataType = null)
  {
    RuleFor(x => x.UniqueName).UniqueName(FieldType.UniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).DisplayName());

    if (dataType.HasValue)
    {
      switch (dataType.Value)
      {
        case DataType.String:
          When(x => x.StringProperties == null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()))
            .Otherwise(() => RuleFor(x => x.StringProperties).Null());
          RuleFor(x => x.TextProperties).Null();
          break;
        case DataType.Text:
          RuleFor(x => x.StringProperties).Null();
          When(x => x.TextProperties == null, () => RuleFor(x => x.TextProperties!).SetValidator(new TextPropertiesValidator()))
            .Otherwise(() => RuleFor(x => x.TextProperties).Null());
          break;
        default:
          throw new DataTypeNotSupportedException(dataType.Value);
      }
    }
    else
    {
      RuleFor(x => x).Must(HaveExactlyOnePropertiesSet)
        .WithErrorCode(nameof(CreateOrReplaceFieldTypeValidator))
        .WithMessage(p => $"Exactly one of the following must be provided: {nameof(p.StringProperties)}, {nameof(p.TextProperties)}.");
    }
  }

  private static bool HaveExactlyOnePropertiesSet(CreateOrReplaceFieldTypePayload payload)
  {
    int count = 0;
    if (payload.StringProperties != null)
    {
      count++;
    }
    if (payload.TextProperties != null)
    {
      count++;
    }
    return count == 1;
  }
}

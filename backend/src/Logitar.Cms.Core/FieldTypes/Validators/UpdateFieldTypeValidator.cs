using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes.Validators;

internal class UpdateFieldTypeValidator : AbstractValidator<UpdateFieldTypePayload>
{
  public UpdateFieldTypeValidator(DataType dataType)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(FieldType.UniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).DisplayName());

    switch (dataType)
    {
      case DataType.String:
        When(x => x.StringProperties == null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.StringProperties).NotNull());
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.Text:
        RuleFor(x => x.StringProperties).Null();
        When(x => x.TextProperties == null, () => RuleFor(x => x.TextProperties!).SetValidator(new TextPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.TextProperties).NotNull());
        break;
      default:
        throw new DataTypeNotSupportedException(dataType);
    }
  }
}

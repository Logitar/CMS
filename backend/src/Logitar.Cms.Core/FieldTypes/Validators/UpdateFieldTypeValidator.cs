using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class UpdateFieldTypeValidator : AbstractValidator<UpdateFieldTypePayload>
{
  public UpdateFieldTypeValidator(IUniqueNameSettings uniqueNameSettings, DataType dataType)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(uniqueNameSettings)));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    switch (dataType)
    {
      case DataType.Boolean:
        When(x => x.BooleanProperties != null, () => RuleFor(x => x.BooleanProperties!).SetValidator(new BooleanPropertiesValidator()));
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.DateTime:
        RuleFor(x => x.BooleanProperties).Null();
        When(x => x.DateTimeProperties != null, () => RuleFor(x => x.DateTimeProperties!).SetValidator(new DateTimePropertiesValidator()));
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.Number:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        When(x => x.NumberProperties != null, () => RuleFor(x => x.NumberProperties!).SetValidator(new NumberPropertiesValidator()));
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.String:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        When(x => x.StringProperties != null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()));
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.Text:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        When(x => x.TextProperties != null, () => RuleFor(x => x.TextProperties!).SetValidator(new TextPropertiesValidator()));
        break;
      default:
        throw new DataTypeNotSupportedException(dataType);
    }
  }
}

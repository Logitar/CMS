using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Fields.Validators;

public class ReplaceFieldTypeValidator : AbstractValidator<ReplaceFieldTypePayload>
{
  public ReplaceFieldTypeValidator(DataType dataType, IUniqueNameSettings uniqueNameSettings)
  {
    switch (dataType)
    {
      case DataType.Boolean:
        When(x => x.BooleanProperties != null, () => RuleFor(x => x.BooleanProperties!).SetValidator(new BooleanPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.BooleanProperties).NotNull());
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.DateTime:
        RuleFor(x => x.BooleanProperties).Null();
        When(x => x.DateTimeProperties != null, () => RuleFor(x => x.DateTimeProperties!).SetValidator(new DateTimePropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.DateTimeProperties).NotNull());
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.Number:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        When(x => x.NumberProperties != null, () => RuleFor(x => x.NumberProperties!).SetValidator(new NumberPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.NumberProperties).NotNull());
        RuleFor(x => x.StringProperties).Null();
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.String:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        When(x => x.StringProperties != null, () => RuleFor(x => x.StringProperties!).SetValidator(new StringPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.StringProperties).NotNull());
        RuleFor(x => x.TextProperties).Null();
        break;
      case DataType.Text:
        RuleFor(x => x.BooleanProperties).Null();
        RuleFor(x => x.DateTimeProperties).Null();
        RuleFor(x => x.NumberProperties).Null();
        RuleFor(x => x.StringProperties).Null();
        When(x => x.TextProperties != null, () => RuleFor(x => x.TextProperties!).SetValidator(new TextPropertiesValidator()))
          .Otherwise(() => RuleFor(x => x.TextProperties).NotNull());
        break;
      default:
        throw new DataTypeNotSupportedException(dataType);
    }

    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}

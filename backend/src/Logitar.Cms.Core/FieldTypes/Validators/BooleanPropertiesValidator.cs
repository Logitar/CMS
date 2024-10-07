using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class BooleanPropertiesValidator : AbstractValidator<IBooleanProperties>
{
  public BooleanPropertiesValidator()
  {
  }
}

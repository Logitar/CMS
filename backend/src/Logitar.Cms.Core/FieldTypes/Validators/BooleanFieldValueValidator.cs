using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class BooleanFieldValueValidator : AbstractValidator<bool>
{
  public BooleanFieldValueValidator(IBooleanProperties properties)
  {
  }
}

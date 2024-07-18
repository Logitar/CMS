using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class NumberFieldValueValidator : AbstractValidator<double>
{
  public NumberFieldValueValidator(INumberProperties properties)
  {
    // TODO(fpion): implement
  }
}

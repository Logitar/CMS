using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Core.FieldTypes.Validators;

public class DateTimeFieldValueValidator : AbstractValidator<string>
{
  public DateTimeFieldValueValidator(IDateTimeProperties properties)
  {
    // TODO(fpion): implement
  }
}

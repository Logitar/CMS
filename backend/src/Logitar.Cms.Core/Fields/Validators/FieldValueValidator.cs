using FluentValidation;
using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Validators;

internal class FieldValueValidator : AbstractValidator<FieldValue>
{
  public FieldValueValidator()
  {
    RuleFor(x => x.Value).NotEmpty();
  }
}

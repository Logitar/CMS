using FluentValidation;
using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Validators;

internal class FieldValueUpdateValidator : AbstractValidator<FieldValueUpdate>
{
  public FieldValueUpdateValidator()
  {
    When(x => x.Value != null, () => RuleFor(x => x.Value).NotEmpty());
  }
}

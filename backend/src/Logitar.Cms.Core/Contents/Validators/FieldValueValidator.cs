using FluentValidation;
using Logitar.Cms.Contracts.Contents;

namespace Logitar.Cms.Core.Contents.Validators;

public class FieldValueValidator : AbstractValidator<FieldValue>
{
  public FieldValueValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Value).NotEmpty();
  }
}

using FluentValidation;
using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Validators;

internal class SelectOptionValidator : AbstractValidator<SelectOptionModel>
{
  public SelectOptionValidator()
  {
    RuleFor(x => x.Text).NotEmpty();
  }
}

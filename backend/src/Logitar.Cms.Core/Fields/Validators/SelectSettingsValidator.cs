using FluentValidation;
using Logitar.Cms.Core.Fields.Models;

namespace Logitar.Cms.Core.Fields.Validators;

internal class SelectSettingsValidator : AbstractValidator<SelectSettingsModel>
{
  public SelectSettingsValidator()
  {
    RuleForEach(x => x.Options).SetValidator(new SelectOptionValidator());
  }
}

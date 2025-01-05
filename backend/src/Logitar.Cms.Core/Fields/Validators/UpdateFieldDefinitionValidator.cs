using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields.Validators;

internal class UpdateFieldDefinitionValidator : AbstractValidator<UpdateFieldDefinitionPayload>
{
  public UpdateFieldDefinitionValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder?.Value), () => RuleFor(x => x.Placeholder!.Value!).Placeholder());
  }
}

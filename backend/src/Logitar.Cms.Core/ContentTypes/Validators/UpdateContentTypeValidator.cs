using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Core.ContentTypes.Validators;

internal class UpdateContentTypeValidator : AbstractValidator<UpdateContentTypePayload>
{
  public UpdateContentTypeValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).DisplayName());
  }
}

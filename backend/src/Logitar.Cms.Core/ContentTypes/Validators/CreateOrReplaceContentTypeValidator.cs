using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;

namespace Logitar.Cms.Core.ContentTypes.Validators;

internal class CreateOrReplaceContentTypeValidator : AbstractValidator<CreateOrReplaceContentTypePayload>
{
  public CreateOrReplaceContentTypeValidator()
  {
    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).DisplayName());
  }
}

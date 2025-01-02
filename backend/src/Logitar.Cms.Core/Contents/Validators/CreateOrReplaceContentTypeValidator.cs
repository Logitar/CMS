using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents.Validators;

internal class CreateOrReplaceContentTypeValidator : AbstractValidator<CreateOrReplaceContentTypePayload>
{
  public CreateOrReplaceContentTypeValidator()
  {
    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}

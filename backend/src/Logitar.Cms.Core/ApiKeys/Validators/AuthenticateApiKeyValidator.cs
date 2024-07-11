using FluentValidation;
using Logitar.Cms.Contracts.ApiKeys;

namespace Logitar.Cms.Core.ApiKeys.Validators;

public class AuthenticateApiKeyValidator : AbstractValidator<AuthenticateApiKeyPayload>
{
  public AuthenticateApiKeyValidator()
  {
    RuleFor(x => x.XApiKey).NotEmpty();
  }
}

using FluentValidation;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Users.Validators;

internal class AuthenticateUserValidator : AbstractValidator<AuthenticateUserPayload>
{
  public AuthenticateUserValidator()
  {
    RuleFor(x => x.Username).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}

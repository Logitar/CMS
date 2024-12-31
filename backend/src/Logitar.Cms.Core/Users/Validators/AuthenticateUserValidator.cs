using FluentValidation;
using Logitar.Cms.Core.Users.Models;

namespace Logitar.Cms.Core.Users.Validators;

internal class AuthenticateUserValidator : AbstractValidator<AuthenticateUserPayload>
{
  public AuthenticateUserValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}

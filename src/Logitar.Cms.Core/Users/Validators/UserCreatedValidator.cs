using FluentValidation;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Events;

namespace Logitar.Cms.Core.Users.Validators;

internal class UserCreatedValidator : UserSavedValidator<UserCreated>
{
  public UserCreatedValidator(IUsernameSettings usernameSettings) : base()
  {
    RuleFor(x => x.Username).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Username(usernameSettings);
  }
}

using FluentValidation;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users.Events;

namespace Logitar.Cms.Core.Users.Validators;

internal class UserCreatedValidator : AbstractValidator<UserCreated>
{
  public UserCreatedValidator(IUsernameSettings usernameSettings)
  {
    RuleFor(x => x.Username).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Username(usernameSettings);

    RuleFor(x => x.FirstName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.LastName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.FullName).Equal(e => UserAggregate.GetFullName(e.FirstName, e.LastName));

    RuleFor(x => x.Locale).Locale();
  }
}

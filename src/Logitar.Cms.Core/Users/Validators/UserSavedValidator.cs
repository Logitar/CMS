using FluentValidation;
using Logitar.Cms.Core.Users.Events;

namespace Logitar.Cms.Core.Users.Validators;

internal abstract class UserSavedValidator<T> : AbstractValidator<T> where T : UserSaved
{
  protected UserSavedValidator()
  {
    RuleFor(x => x.FirstName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.LastName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.FullName).Equal(e => UserAggregate.GetFullName(e.FirstName, e.LastName));

    RuleFor(x => x.Locale).Locale();
  }
}

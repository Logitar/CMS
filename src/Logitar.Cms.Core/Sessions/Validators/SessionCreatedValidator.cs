using FluentValidation;
using Logitar.Cms.Core.Sessions.Events;

namespace Logitar.Cms.Core.Sessions.Validators;

internal class SessionCreatedValidator : AbstractValidator<SessionCreated>
{
  public SessionCreatedValidator()
  {
    RuleFor(x => x.IpAddress).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.AdditionalInformation).NullOrNotEmpty();
  }
}

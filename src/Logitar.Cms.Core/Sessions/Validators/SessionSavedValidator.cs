using FluentValidation;
using Logitar.Cms.Core.Sessions.Events;

namespace Logitar.Cms.Core.Sessions.Validators;

internal abstract class SessionSavedValidator<T> : AbstractValidator<T> where T : SessionSaved
{
  protected SessionSavedValidator()
  {
    RuleFor(x => x.IpAddress).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.AdditionalInformation).NullOrNotEmpty();
  }
}

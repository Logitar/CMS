using Logitar.Cms.Core.Sessions.Events;

namespace Logitar.Cms.Core.Sessions.Validators;

internal class SessionCreatedValidator : SessionSavedValidator<SessionCreated>
{
  public SessionCreatedValidator() : base() { }
}

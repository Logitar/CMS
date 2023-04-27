using Logitar.Cms.Core.Sessions.Events;

namespace Logitar.Cms.Core.Sessions.Validators;

internal class SessionRefreshedValidator : SessionSavedValidator<SessionRefreshed>
{
  public SessionRefreshedValidator() : base() { }
}

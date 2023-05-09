namespace Logitar.Cms.Contracts.Sessions;

public interface ISessionService
{
  Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken = default);
}

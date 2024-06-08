using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, Session?>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandHandler(ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionId sessionId = new(command.Id);
    SessionAggregate? session = await _sessionRepository.LoadAsync(sessionId, cancellationToken);
    if (session == null)
    {
      return null;
    }

    UserAggregate user = await _userRepository.LoadAsync(session, cancellationToken);
    if (user.TenantId != null)
    {
      return null;
    }

    session.SignOut(command.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
} // TODO(fpion): Integration Tests

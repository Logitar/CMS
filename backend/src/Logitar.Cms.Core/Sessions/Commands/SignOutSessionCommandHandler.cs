using Logitar.Cms.Contracts.Sessions;
using Logitar.Identity.Domain.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, Session?>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutSessionCommandHandler(ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionId id = new(command.Id);
    SessionAggregate? session = await _sessionRepository.LoadAsync(id, cancellationToken);
    if (session == null)
    {
      return null;
    }

    session.SignOut(command.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}

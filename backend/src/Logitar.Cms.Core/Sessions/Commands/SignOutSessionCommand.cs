using Logitar.Cms.Core.Sessions.Models;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record SignOutSessionCommand(Guid Id) : IRequest<SessionModel?>;

internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, SessionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public SignOutSessionCommandHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<SessionModel?> Handle(SignOutSessionCommand command, CancellationToken cancellationToken)
  {
    SessionId sessionId = new(tenantId: null, new EntityId(command.Id));
    Session? session = await _sessionRepository.LoadAsync(sessionId, cancellationToken);
    if (session == null)
    {
      return null;
    }

    session.SignOut(_applicationContext.ActorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}

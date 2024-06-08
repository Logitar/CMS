using Logitar.Cms.Contracts.Users;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

internal class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand, User?>
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandler(ISessionRepository sessionRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _sessionRepository = sessionRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(SignOutUserCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.Id);
    UserAggregate? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != null)
    {
      return null;
    }

    ActorId actorId = command.ActorId;
    IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadActiveAsync(user, cancellationToken);
    foreach (SessionAggregate session in sessions)
    {
      session.SignOut(actorId);
    }

    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
} // TODO(fpion): Integration Tests

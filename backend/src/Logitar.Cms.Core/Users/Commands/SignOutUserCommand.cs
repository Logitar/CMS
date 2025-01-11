using Logitar.Cms.Core.Users.Models;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record SignOutUserCommand(Guid Id) : IRequest<UserModel?>;

internal class SignOutUserCommandHandler : IRequestHandler<SignOutUserCommand, UserModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandHandler(
    IApplicationContext applicationContext,
    ISessionRepository sessionRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _sessionRepository = sessionRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(SignOutUserCommand command, CancellationToken cancellationToken)
  {
    UserId userId = new(tenantId: null, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;
    IReadOnlyCollection<Session> sessions = await _sessionRepository.LoadActiveAsync(user, cancellationToken);
    foreach (Session session in sessions)
    {
      session.SignOut(actorId);
    }
    await _sessionRepository.SaveAsync(sessions, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

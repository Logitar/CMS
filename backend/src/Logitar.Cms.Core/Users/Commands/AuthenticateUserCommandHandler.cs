using FluentValidation;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Users.Queries;
using Logitar.Cms.Core.Users.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, User>
{
  private readonly ISender _sender;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(ISender sender, IUserManager userManager, IUserQuerier userQuerier)
  {
    _sender = sender;
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<User> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FindUserQuery query = new(payload.Username, command.UserSettings, nameof(payload.Username));
    UserAggregate user = await _sender.Send(query, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

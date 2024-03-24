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
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandHandler(ISender sender, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _sender = sender;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FindUserQuery query = new(payload.Username, command.UserSettings, nameof(payload.Username));
    UserAggregate user = await _sender.Send(query, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    user.Authenticate(payload.Password, actorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

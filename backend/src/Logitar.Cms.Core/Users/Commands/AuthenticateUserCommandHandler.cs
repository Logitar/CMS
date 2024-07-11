using FluentValidation;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Users.Queries;
using Logitar.Cms.Core.Users.Validators;
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

    UserAggregate user = await _sender.Send(new FindUserQuery(payload.UniqueName, nameof(payload.UniqueName)), cancellationToken);
    user.Authenticate(payload.Password);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

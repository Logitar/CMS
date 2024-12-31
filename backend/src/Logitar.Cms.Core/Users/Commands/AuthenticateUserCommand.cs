using FluentValidation;
using Logitar.Cms.Core.Users.Models;
using Logitar.Cms.Core.Users.Validators;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : IRequest<UserModel>;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserModel>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandHandler(IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FoundUsers users = await _userManager.FindAsync(tenantId: null, payload.UniqueName, cancellationToken);
    if (users.Count > 1)
    {
      throw TooManyResultsException<User>.ExpectedSingle(users.Count);
    }
    User user = users.SingleOrDefault() ?? throw new UserNotFoundException(payload.UniqueName, nameof(payload.UniqueName));
    user.Authenticate(payload.Password);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

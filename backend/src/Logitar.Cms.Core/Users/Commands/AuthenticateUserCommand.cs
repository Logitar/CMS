using FluentValidation;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Users.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : Activity, IRequest<UserModel>;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserModel>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IUserManager userManager, IUserQuerier userQuerier)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    IUserSettings userSettings = command.GetUserSettings();
    FoundUsers users = await _userManager.FindAsync(tenantId: null, payload.Username, userSettings, cancellationToken);
    if (users.Count > 1)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }
    UserAggregate user = users.SingleOrDefault() ?? throw new NotImplementedException(); // TODO(fpion): implement

    user.Authenticate(payload.Password);

    ActorId actorId = new(user.Id.Value);
    await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}

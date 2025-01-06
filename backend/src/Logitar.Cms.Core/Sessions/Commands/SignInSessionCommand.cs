using FluentValidation;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Sessions.Validators;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record SignInSessionCommand(SignInSessionPayload Payload) : IRequest<SessionModel>;

internal class SignInSessionCommandHandler : IRequestHandler<SignInSessionCommand, SessionModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;
  private readonly IUserRepository _userRepository;

  public SignInSessionCommandHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserManager userManager,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
    _userRepository = userRepository;
  }

  public async Task<SessionModel> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    FoundUsers users = await _userManager.FindAsync(tenantId: null, payload.UniqueName, cancellationToken);
    if (users.Count > 1)
    {
      throw TooManyResultsException<User>.ExpectedSingle(users.Count);
    }
    User user = users.SingleOrDefault() ?? throw new UserNotFoundException(payload.UniqueName, nameof(payload.UniqueName));

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }
    ActorId actorId = _applicationContext.ActorId ?? new(user.Id.Value);
    EntityId? entityId = payload.Id.HasValue ? new EntityId(payload.Id.Value) : null;
    Session session = user.SignIn(payload.Password, secret, actorId, entityId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }
    session.Update(actorId);

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel model = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString != null)
    {
      model.RefreshToken = new RefreshToken(model.Id, secretString).Encode();
    }
    return model;
  }
}

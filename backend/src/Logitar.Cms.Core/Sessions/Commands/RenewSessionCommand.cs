using FluentValidation;
using Logitar.Cms.Core.Sessions.Models;
using Logitar.Cms.Core.Sessions.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

/// <exception cref="IncorrectSessionSecretException"></exception>
/// <exception cref="InvalidRefreshTokenException"></exception>
/// <exception cref="SessionIsNotActiveException"></exception>
/// <exception cref="SessionIsNotPersistentException"></exception>
/// <exception cref="SessionNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record RenewSessionCommand(RenewSessionPayload Payload) : IRequest<SessionModel>;

internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, SessionModel>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RenewSessionCommandHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<SessionModel> Handle(RenewSessionCommand command, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = command.Payload;
    new RenewSessionValidator().ValidateAndThrow(payload);

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(payload.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, nameof(payload.RefreshToken), innerException);
    }

    SessionId sessionId = new(tenantId: null, new EntityId(refreshToken.SessionId));
    Session session = await _sessionRepository.LoadAsync(sessionId, cancellationToken)
      ?? throw new SessionNotFoundException(sessionId, nameof(payload.RefreshToken));

    ActorId? actorId = _applicationContext.ActorId;

    Password newSecret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    session.Renew(refreshToken.Secret, newSecret, actorId);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(new Identifier(customAttribute.Key), customAttribute.Value);
    }
    session.Update(actorId);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    SessionModel model = await _sessionQuerier.ReadAsync(session, cancellationToken);
    model.RefreshToken = new RefreshToken(session.EntityId.ToGuid(), secretString).Encode();
    return model;
  }
}

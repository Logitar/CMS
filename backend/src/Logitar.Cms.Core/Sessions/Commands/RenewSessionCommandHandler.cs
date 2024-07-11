using FluentValidation;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Sessions.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class RenewSessionCommandHandler : IRequestHandler<RenewSessionCommand, Session>
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RenewSessionCommandHandler(IPasswordManager passwordManager, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(RenewSessionCommand command, CancellationToken cancellationToken)
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

    SessionAggregate session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new SessionNotFoundException(refreshToken.Id, nameof(payload.RefreshToken));

    Password newSecret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    session.Renew(refreshToken.Secret, newSecret);

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(new ActorId(session.UserId.Value));

    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    result.RefreshToken = RefreshToken.Encode(session, secretString);
    return result;
  }
}

using FluentValidation;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Sessions.Validators;
using Logitar.Cms.Core.Users.Queries;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class SignInSessionCommandHandler : IRequestHandler<SignInSessionCommand, Session>
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISender _sender;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignInSessionCommandHandler(
    IPasswordManager passwordManager,
    ISender sender,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _passwordManager = passwordManager;
    _sender = sender;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    UserAggregate user = await _sender.Send(new FindUserQuery(payload.UniqueName, nameof(payload.UniqueName)), cancellationToken);

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    SessionAggregate session = user.SignIn(payload.Password, secret);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(new ActorId(user.Id.Value));

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString != null)
    {
      result.RefreshToken = RefreshToken.Encode(session, secretString).ToString();
    }
    return result;
  }
}

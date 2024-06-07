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
  private readonly IUserManager _userManager;

  public SignInSessionCommandHandler(IPasswordManager passwordManager, ISender sender,
    ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserManager userManager)
  {
    _passwordManager = passwordManager;
    _sender = sender;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<Session> Handle(SignInSessionCommand command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    FindUserQuery query = new(payload.Username, command.UserSettings, nameof(payload.Username));
    UserAggregate user = await _sender.Send(query, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);

    SessionAggregate session = user.SignIn(payload.Password, secret, actorId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(actorId);

    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session result = await _sessionQuerier.ReadAsync(session, cancellationToken);
    result.RefreshToken = RefreshToken.Encode(session, secretString);
    return result;
  }
} // TODO(fpion): Integration Tests

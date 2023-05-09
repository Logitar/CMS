using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class SignInHandler : IRequestHandler<SignIn, Session>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignInHandler(ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(SignIn request, CancellationToken cancellationToken)
  {
    SignInInput input = request.Input;

    UserAggregate user = await _userRepository.LoadAsync(input.Username, cancellationToken)
      ?? throw new InvalidCredentialsException($"The user '{input.Username}' could not be found.");

    SessionAggregate session = user.SignIn(input.Password, input.Remember, input.IpAddress, input.AdditionalInformation);

    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session output = await _sessionQuerier.GetAsync(session, cancellationToken);
    output.RefreshToken = session.RefreshToken?.ToString();

    return output;
  }
}

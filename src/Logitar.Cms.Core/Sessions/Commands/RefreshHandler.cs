using Logitar.Cms.Contracts.Sessions;
using Logitar.Cms.Core.Users;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal class RefreshHandler : IRequestHandler<Refresh, Session>
{
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;

  public RefreshHandler(ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
  }

  public async Task<Session> Handle(Refresh request, CancellationToken cancellationToken)
  {
    RefreshInput input = request.Input;

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Parse(input.RefreshToken);
    }
    catch (Exception innerException)
    {
      throw new InvalidCredentialsException($"The value '{input.RefreshToken}' is not a valid refresh token.", innerException);
    }

    SessionAggregate session = await _sessionRepository.LoadAsync(refreshToken.Id, cancellationToken)
      ?? throw new InvalidCredentialsException($"The session aggregate '{refreshToken.Id}' could not be found.");

    session.Refresh(refreshToken.Secret, input.IpAddress, input.AdditionalInformation);

    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session output = await _sessionQuerier.GetAsync(session, cancellationToken);
    output.RefreshToken = session.RefreshToken?.ToString();

    return output;
  }
}

using Logitar.Cms.Core.Sessions.Models;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Queries;

public record ReadSessionQuery(Guid Id) : IRequest<SessionModel?>;

internal class ReadSessionQueryHandler : IRequestHandler<ReadSessionQuery, SessionModel?>
{
  private readonly ISessionQuerier _sessionQuerier;

  public ReadSessionQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SessionModel?> Handle(ReadSessionQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

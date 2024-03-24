using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Shared;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

internal class ReadUserQueryHandler : IRequestHandler<ReadUserQuery, User?>
{
  private readonly IUserQuerier _userQuerier;

  public ReadUserQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<User?> Handle(ReadUserQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, User> users = new(capacity: 2);

    if (query.Id.HasValue)
    {
      User? user = await _userQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Username))
    {
      User? user = await _userQuerier.ReadAsync(query.Username, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<User>.ExpectedSingle(users.Count);
    }

    return users.SingleOrDefault().Value;
  }
}

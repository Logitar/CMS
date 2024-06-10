using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

internal class FindUserQueryHandler : IRequestHandler<FindUserQuery, UserAggregate>
{
  private readonly IUserRepository _userRepository;

  public FindUserQueryHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<UserAggregate> Handle(FindUserQuery query, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = query.UserSettings;

    Dictionary<UserId, UserAggregate> users = new(capacity: 3);

    if (query.IncludeId && Guid.TryParse(query.User, out Guid id))
    {
      UserAggregate? user = await _userRepository.LoadAsync(new UserId(id), cancellationToken);
      if (user != null && user.TenantId == null)
      {
        users[user.Id] = user;
      }
    }

    UniqueNameUnit? uniqueName = null;
    try
    {
      uniqueName = new(userSettings.UniqueName, query.User);
    }
    catch (Exception)
    {
    }
    if (uniqueName != null)
    {
      UserAggregate? user = await _userRepository.LoadAsync(tenantId: null, uniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (userSettings.RequireUniqueEmail)
    {
      EmailUnit? email = null;
      try
      {
        email = new(query.User);
      }
      catch (Exception)
      {
      }
      if (email != null)
      {
        IEnumerable<UserAggregate> foundUsers = await _userRepository.LoadAsync(tenantId: null, email, cancellationToken);
        if (foundUsers.Count() == 1)
        {
          UserAggregate user = foundUsers.Single();
          users[user.Id] = user;
        }
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<UserAggregate>(expectedCount: 1, actualCount: users.Count);
    }

    return users.Values.SingleOrDefault() ?? throw new UserNotFoundException(query.User, query.PropertyName);
  }
}

using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Configurations;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

internal class FindUserQueryHandler : IRequestHandler<FindUserQuery, UserAggregate>
{
  private readonly ICacheService _cacheService;
  private readonly IUserRepository _userRepository;

  public FindUserQueryHandler(ICacheService cacheService, IUserRepository userRepository)
  {
    _cacheService = cacheService;
    _userRepository = userRepository;
  }

  public async Task<UserAggregate> Handle(FindUserQuery query, CancellationToken cancellationToken)
  {
    UserAggregate? user = await TryFindByUniqueNameAsync(query.UniqueName, cancellationToken);
    if (user != null)
    {
      return user;
    }

    if (_cacheService.Configuration != null && _cacheService.Configuration.RequireUniqueEmail)
    {
      user = await TryFindByEmailAddressAsync(query.UniqueName, cancellationToken);
      if (user != null)
      {
        return user;
      }
    }

    throw new UserNotFoundException(query.UniqueName, query.PropertyName);
  }

  private async Task<UserAggregate?> TryFindByUniqueNameAsync(string value, CancellationToken cancellationToken)
  {
    try
    {
      UniqueNameUnit uniqueName = new(new ReadOnlyUniqueNameSettings(allowedCharacters: null), value);
      return await _userRepository.LoadAsync(tenantId: null, uniqueName, cancellationToken);
    }
    catch (Exception)
    {
      return null;
    }
  }

  private async Task<UserAggregate?> TryFindByEmailAddressAsync(string address, CancellationToken cancellationToken)
  {
    try
    {
      EmailUnit email = new(address);
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId: null, email, cancellationToken);
      return users.Count() == 1 ? users.Single() : null;
    }
    catch (Exception)
    {
      return null;
    }
  }
}

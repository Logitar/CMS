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
    UniqueNameUnit uniqueName = new(query.UserSettings.UniqueName, query.Username); // TODO(fpion): will throw if invalid
    UserAggregate? user = await _userRepository.LoadAsync(tenantId: null, uniqueName, cancellationToken);
    if (user == null && query.UserSettings.RequireUniqueEmail)
    {
      EmailUnit email = new(query.Username); // TODO(fpion): will throw if invalid
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId: null, email, cancellationToken);
      if (users.Count() == 1)
      {
        user = users.Single();
      }
    }

    return user ?? throw new NotImplementedException(); // TODO(fpion): UserNotFoundException (InvalidCredentialsException)
  }
}

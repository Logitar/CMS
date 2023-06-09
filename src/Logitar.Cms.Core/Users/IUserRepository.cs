﻿namespace Logitar.Cms.Core.Users;

public interface IUserRepository
{
  Task<UserAggregate?> LoadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<UserAggregate>> LoadAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
  Task<UserAggregate?> LoadAsync(string username, CancellationToken cancellationToken = default);
  Task SaveAsync(UserAggregate user, CancellationToken cancellationToken = default);
}

using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

public record FindUserQuery(string Username, IUserSettings UserSettings, string PropertyName) : IRequest<UserAggregate>;

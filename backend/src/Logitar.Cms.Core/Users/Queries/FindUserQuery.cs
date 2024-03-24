using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

internal record FindUserQuery(string User, IUserSettings UserSettings, string? PropertyName, bool IncludeId = false) : IRequest<UserAggregate>;

using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

public record FindUserQuery(string User, IUserSettings UserSettings, string? PropertyName = null, bool IncludeId = false) : IRequest<UserAggregate>;

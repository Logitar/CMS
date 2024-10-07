using Logitar.Identity.Domain.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

public record FindUserQuery(string UniqueName, string? PropertyName) : IRequest<UserAggregate>;

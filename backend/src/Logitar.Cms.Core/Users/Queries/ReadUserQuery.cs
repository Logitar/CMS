using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Queries;

public record ReadUserQuery(Guid? Id, string? Username) : Activity, IRequest<User?>;

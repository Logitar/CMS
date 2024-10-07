using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record SignOutUserCommand(Guid Id) : Activity, IRequest<User?>;

using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : Activity, IRequest<User>;

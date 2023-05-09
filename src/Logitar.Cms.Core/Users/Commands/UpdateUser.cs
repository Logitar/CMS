using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

internal record UpdateUser(Guid Id, UpdateUserInput Input) : IRequest<User>;

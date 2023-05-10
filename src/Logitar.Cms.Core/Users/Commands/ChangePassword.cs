using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

internal record ChangePassword(Guid Id, ChangePasswordInput Input) : IRequest<User>;

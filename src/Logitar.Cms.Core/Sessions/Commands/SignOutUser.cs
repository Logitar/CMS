using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal record SignOutUser(Guid Id) : IRequest<IEnumerable<Session>>;

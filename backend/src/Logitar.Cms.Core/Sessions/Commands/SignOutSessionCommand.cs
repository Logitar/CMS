using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record SignOutSessionCommand(Guid Id) : Activity, IRequest<Session?>;

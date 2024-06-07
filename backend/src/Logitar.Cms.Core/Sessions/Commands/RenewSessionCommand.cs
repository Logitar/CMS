using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record RenewSessionCommand(RenewSessionPayload Payload) : Activity, IRequest<Session>;

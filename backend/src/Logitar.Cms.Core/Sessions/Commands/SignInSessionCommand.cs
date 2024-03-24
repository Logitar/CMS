using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record SignInSessionCommand(SignInSessionPayload Payload) : Activity, IRequest<Session>;

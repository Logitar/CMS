using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

public record SignInSessionCommand(SignInSessionPayload Payload) : Activity, IRequest<Session>
{
  public override IActivity Anonymize()
  {
    SignInSessionCommand command = this.DeepClone();
    command.Payload.Password = command.Payload.Password.Mask();
    return command;
  }
}

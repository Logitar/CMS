using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : Activity, IRequest<User>
{
  public override IActivity Anonymize()
  {
    AuthenticateUserCommand command = this.DeepClone();
    command.Payload.Password = command.Payload.Password.Mask();
    return command;
  }
}

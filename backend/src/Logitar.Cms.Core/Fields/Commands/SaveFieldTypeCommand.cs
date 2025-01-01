using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record SaveFieldTypeCommand(FieldType FieldType) : IRequest;

internal class SaveFieldTypeCommandHandler : IRequestHandler<SaveFieldTypeCommand>
{
  public Task Handle(SaveFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldType fieldType = command.FieldType;

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}

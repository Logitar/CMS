using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record UpdateFieldTypeCommand(Guid Id, UpdateFieldTypePayload Payload) : Activity, IRequest<FieldTypeModel?>;

public class UpdateFieldTypeCommandHandler : IRequestHandler<UpdateFieldTypeCommand, FieldTypeModel?>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ISender _sender;

  public UpdateFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository, ISender sender)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
    _sender = sender;
  }

  public async Task<FieldTypeModel?> Handle(UpdateFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = new(command.Id);
    FieldType? fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }

    UpdateFieldTypePayload payload = command.Payload;
    new UpdateFieldTypeValidator(fieldType.DataType).ValidateAndThrow(command.Payload);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      fieldType.SetUniqueName(payload.UniqueName);
    }
    if (payload.DisplayName != null)
    {
      fieldType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      fieldType.Description = Description.TryCreate(payload.Description.Value);
    }

    ActorId actorId = command.GetActorId();
    fieldType.Update(actorId);

    if (payload.StringProperties != null)
    {
      fieldType.SetProperties(new StringProperties(payload.StringProperties), actorId);
    }
    if (payload.TextProperties != null)
    {
      fieldType.SetProperties(new TextProperties(payload.TextProperties), actorId);
    }

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}

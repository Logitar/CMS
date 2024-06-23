using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

internal class UpdateFieldTypeCommandHandler : IRequestHandler<UpdateFieldTypeCommand, FieldType?>
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

  public async Task<FieldType?> Handle(UpdateFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeId id = new(command.Id);
    FieldTypeAggregate? fieldType = await _fieldTypeRepository.LoadAsync(id, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;

    UpdateFieldTypePayload payload = command.Payload;
    new UpdateFieldTypeValidator(fieldType.DataType, uniqueNameSettings).ValidateAndThrow(payload);

    UniqueNameUnit? uniqueName = UniqueNameUnit.TryCreate(uniqueNameSettings, payload.UniqueName);
    if (uniqueName != null)
    {
      fieldType.UniqueName = uniqueName;
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName?.Value);
    if (displayName != null)
    {
      fieldType.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description?.Value);
    if (description != null)
    {
      fieldType.Description = description;
    }

    if (payload.BooleanProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyBooleanProperties(payload.BooleanProperties), command.ActorId);
    }
    if (payload.DateTimeProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyDateTimeProperties(payload.DateTimeProperties), command.ActorId);
    }
    if (payload.NumberProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyNumberProperties(payload.NumberProperties), command.ActorId);
    }
    if (payload.StringProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyStringProperties(payload.StringProperties), command.ActorId);
    }
    if (payload.TextProperties != null)
    {
      fieldType.SetProperties(new ReadOnlyTextProperties(payload.TextProperties), command.ActorId);
    }

    fieldType.Update(command.ActorId);

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}

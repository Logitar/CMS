using FluentValidation;
using Logitar.Cms.Contracts.Fields;
using Logitar.Cms.Core.Fields.Properties;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

internal class ReplaceFieldTypeCommandHandler : IRequestHandler<ReplaceFieldTypeCommand, FieldType?>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ISender _sender;

  public ReplaceFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository, ISender sender)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
    _sender = sender;
  }

  public async Task<FieldType?> Handle(ReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    FieldTypeId id = new(command.Id);
    FieldTypeAggregate? fieldType = await _fieldTypeRepository.LoadAsync(id, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }
    FieldTypeAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _fieldTypeRepository.LoadAsync(fieldType.Id, command.Version.Value, cancellationToken);
    }

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;

    ReplaceFieldTypePayload payload = command.Payload;
    new ReplaceFieldTypeValidator(fieldType.DataType, uniqueNameSettings).ValidateAndThrow(payload);

    UniqueNameUnit uniqueName = new(uniqueNameSettings, payload.UniqueName);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      fieldType.UniqueName = uniqueName;
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      fieldType.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
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

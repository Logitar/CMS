using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

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

    IUniqueNameSettings uniqueNameSettings = FieldTypeAggregate.UniqueNameSettings;

    ReplaceFieldTypePayload payload = command.Payload;
    new ReplaceFieldTypeValidator(uniqueNameSettings, fieldType.DataType).ValidateAndThrow(payload);

    FieldTypeAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _fieldTypeRepository.LoadAsync(id, command.Version.Value, cancellationToken);
    }

    UniqueNameUnit uniqueName = new(uniqueNameSettings, payload.UniqueName);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      fieldType.UniqueName = uniqueName;
    }
    if (reference == null || displayName != reference.DisplayName)
    {
      fieldType.DisplayName = displayName;
    }
    if (reference == null || description != reference.Description)
    {
      fieldType.Description = description;
    }
    fieldType.Update(command.ActorId);

    if (payload.BooleanProperties != null)
    {
      ReadOnlyBooleanProperties properties = new(payload.BooleanProperties);
      if (reference == null || properties != reference.Properties)
      {
        fieldType.SetProperties(properties, command.ActorId);
      }
    }
    if (payload.DateTimeProperties != null)
    {
      ReadOnlyDateTimeProperties properties = new(payload.DateTimeProperties);
      if (reference == null || properties != reference.Properties)
      {
        fieldType.SetProperties(properties, command.ActorId);
      }
    }
    if (payload.NumberProperties != null)
    {
      ReadOnlyNumberProperties properties = new(payload.NumberProperties);
      if (reference == null || properties != reference.Properties)
      {
        fieldType.SetProperties(properties, command.ActorId);
      }
    }
    if (payload.StringProperties != null)
    {
      ReadOnlyStringProperties properties = new(payload.StringProperties);
      if (reference == null || properties != reference.Properties)
      {
        fieldType.SetProperties(properties, command.ActorId);
      }
    }
    if (payload.TextProperties != null)
    {
      ReadOnlyTextProperties properties = new(payload.TextProperties);
      if (reference == null || properties != reference.Properties)
      {
        fieldType.SetProperties(properties, command.ActorId);
      }
    }

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}

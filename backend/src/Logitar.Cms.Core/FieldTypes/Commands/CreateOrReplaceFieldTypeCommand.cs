using FluentValidation;
using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Cms.Core.FieldTypes.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.FieldTypes.Commands;

public record CreateOrReplaceFieldTypeResult(FieldTypeModel? FieldType = null, bool Created = false);

public record CreateOrReplaceFieldTypeCommand(Guid? Id, CreateOrReplaceFieldTypePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceFieldTypeResult>;

public class CreateOrReplaceFieldTypeCommandHandler : IRequestHandler<CreateOrReplaceFieldTypeCommand, CreateOrReplaceFieldTypeResult>
{
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly ISender _sender;

  public CreateOrReplaceFieldTypeCommandHandler(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository, ISender sender)
  {
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
    _sender = sender;
  }

  public async Task<CreateOrReplaceFieldTypeResult> Handle(CreateOrReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    bool created = false;
    FieldType? fieldType = await FindAsync(command, cancellationToken);
    if (fieldType == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceFieldTypeResult();
      }

      fieldType = Create(command);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, fieldType, cancellationToken);
    }

    await _sender.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    FieldTypeModel model = await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
    return new CreateOrReplaceFieldTypeResult(model, created);
  }

  private async Task<FieldType?> FindAsync(CreateOrReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    if (command.Id == null)
    {
      return null;
    }

    FieldTypeId fieldTypeId = new(command.Id.Value);
    return await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
  }

  private static FieldType Create(CreateOrReplaceFieldTypeCommand command)
  {
    CreateOrReplaceFieldTypePayload payload = command.Payload;
    new CreateOrReplaceFieldTypeValidator().ValidateAndThrow(payload);

    BaseProperties? properties = GetProperties(payload) ?? throw new InvalidOperationException("The field type properties are required.");
    ActorId actorId = command.GetActorId();
    FieldTypeId? fieldTypeId = command.Id.HasValue ? new(command.Id.Value) : null;
    FieldType fieldType = new(payload.UniqueName, properties, actorId, fieldTypeId)
    {
      DisplayName = DisplayName.TryCreate(payload.DisplayName),
      Description = Description.TryCreate(payload.Description)
    };

    fieldType.Update(actorId);

    return fieldType;
  }
  private static BaseProperties? GetProperties(CreateOrReplaceFieldTypePayload payload)
  {
    List<BaseProperties> properties = new(capacity: 2);
    if (payload.StringProperties != null)
    {
      properties.Add(new StringProperties(payload.StringProperties));
    }
    if (payload.TextProperties != null)
    {
      properties.Add(new TextProperties(payload.TextProperties));
    }
    return properties.Count == 1 ? properties.Single() : null;
  }

  private async Task ReplaceAsync(CreateOrReplaceFieldTypeCommand command, FieldType fieldType, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypePayload payload = command.Payload;
    new CreateOrReplaceFieldTypeValidator(fieldType.DataType).ValidateAndThrow(payload);

    ActorId actorId = command.GetActorId();

    FieldType? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _fieldTypeRepository.LoadAsync(fieldType.Id, command.Version.Value, cancellationToken);
    }
    reference ??= fieldType;

    UniqueName uniqueName = new(FieldType.UniqueNameSettings, payload.UniqueName);
    if (reference.UniqueName != uniqueName)
    {
      fieldType.SetUniqueName(payload.UniqueName);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      fieldType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      fieldType.Description = description;
    }

    fieldType.Update(actorId);

    if (payload.StringProperties != null)
    {
      fieldType.SetProperties(new StringProperties(payload.StringProperties), actorId);
    }
    if (payload.TextProperties != null)
    {
      fieldType.SetProperties(new TextProperties(payload.TextProperties), actorId);
    }
  }
}

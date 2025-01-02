using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record CreateOrReplaceFieldTypeResult(FieldTypeModel? FieldType = null, bool Created = false);

public record CreateOrReplaceFieldTypeCommand(Guid? Id, CreateOrReplaceFieldTypePayload Payload, long? Version) : IRequest<CreateOrReplaceFieldTypeResult>;

internal class CreateOrReplaceFieldTypeCommandHandler : IRequestHandler<CreateOrReplaceFieldTypeCommand, CreateOrReplaceFieldTypeResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IMediator _mediator;

  public CreateOrReplaceFieldTypeCommandHandler(
    IApplicationContext applicationContext,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
    _mediator = mediator;
  }

  public async Task<CreateOrReplaceFieldTypeResult> Handle(CreateOrReplaceFieldTypeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypePayload payload = command.Payload;
    new CreateOrReplaceFieldTypeValidator().ValidateAndThrow(payload);

    UniqueName uniqueName = new(FieldType.UniqueNameSettings, payload.UniqueName);
    ActorId? actorId = _applicationContext.ActorId;

    FieldTypeId? fieldTypeId = null;
    FieldType? fieldType = null;
    if (command.Id.HasValue)
    {
      fieldTypeId = new(command.Id.Value);
      fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId.Value, cancellationToken);
    }

    bool created = false;
    if (fieldType == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceFieldTypeResult();
      }

      FieldTypeSettings settings = GetSettings(payload);
      fieldType = new(uniqueName, settings, actorId, fieldTypeId);
      created = true;
    }

    FieldType reference = (command.Version.HasValue
      ? await _fieldTypeRepository.LoadAsync(fieldType.Id, command.Version.Value, cancellationToken)
      : null) ?? fieldType;

    if (!created)
    {
      if (reference.UniqueName != uniqueName)
      {
        fieldType.SetUniqueName(uniqueName, actorId);
      }

      SetSettings(payload, fieldType, reference, actorId);
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

    await _mediator.Send(new SaveFieldTypeCommand(fieldType), cancellationToken);

    FieldTypeModel model = await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
    return new CreateOrReplaceFieldTypeResult(model, created);
  }

  private static FieldTypeSettings GetSettings(CreateOrReplaceFieldTypePayload payload)
  {
    List<FieldTypeSettings> settings = new(capacity: 5);

    if (payload.Boolean != null)
    {
      settings.Add(new BooleanSettings(payload.Boolean));
    }
    if (payload.DateTime != null)
    {
      settings.Add(new DateTimeSettings(payload.DateTime));
    }
    if (payload.Number != null)
    {
      settings.Add(new NumberSettings(payload.Number));
    }
    if (payload.RichText != null)
    {
      settings.Add(new RichTextSettings(payload.RichText));
    }
    if (payload.String != null)
    {
      settings.Add(new StringSettings(payload.String));
    }

    if (settings.Count < 1)
    {
      throw new ArgumentException("The field type payload did not provide any settings.", nameof(payload));
    }
    else if (settings.Count > 1)
    {
      throw new ArgumentException($"The field type payload provided {settings.Count} settings.", nameof(payload));
    }
    return settings.Single();
  }

  private static void SetSettings(CreateOrReplaceFieldTypePayload payload, FieldType fieldType, FieldType reference, ActorId? actorId)
  {
    if (payload.Boolean != null)
    {
      BooleanSettings settings = new(payload.Boolean);
      if (!reference.Settings.Equals(settings))
      {
        fieldType.SetSettings(settings, actorId);
      }
    }
    if (payload.DateTime != null)
    {
      DateTimeSettings settings = new(payload.DateTime);
      if (!reference.Settings.Equals(settings))
      {
        fieldType.SetSettings(settings, actorId);
      }
    }
    if (payload.Number != null)
    {
      NumberSettings settings = new(payload.Number);
      if (!reference.Settings.Equals(settings))
      {
        fieldType.SetSettings(settings, actorId);
      }
    }
    if (payload.RichText != null)
    {
      RichTextSettings settings = new(payload.RichText);
      if (!reference.Settings.Equals(settings))
      {
        fieldType.SetSettings(settings, actorId);
      }
    }
    if (payload.String != null)
    {
      StringSettings settings = new(payload.String);
      if (!reference.Settings.Equals(settings))
      {
        fieldType.SetSettings(settings, actorId);
      }
    }
  }
}

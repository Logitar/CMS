using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record UpdateFieldTypeCommand(Guid Id, UpdateFieldTypePayload Payload) : IRequest<FieldTypeModel?>;

internal class UpdateFieldTypeCommandHandler : IRequestHandler<UpdateFieldTypeCommand, FieldTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFieldTypeManager _fieldTypeManager;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public UpdateFieldTypeCommandHandler(
    IApplicationContext applicationContext,
    IFieldTypeManager fieldTypeManager,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    _applicationContext = applicationContext;
    _fieldTypeManager = fieldTypeManager;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<FieldTypeModel?> Handle(UpdateFieldTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateFieldTypePayload payload = command.Payload;
    new UpdateFieldTypeValidator().ValidateAndThrow(payload);

    FieldTypeId fieldTypeId = new(command.Id);
    FieldType? fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(FieldType.UniqueNameSettings, payload.UniqueName);
      fieldType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      fieldType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      fieldType.Description = Description.TryCreate(payload.Description.Value);
    }

    fieldType.Update(actorId);

    SetSettings(payload, fieldType, actorId);

    await _fieldTypeManager.SaveAsync(fieldType, cancellationToken);

    return await _fieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }

  private static void SetSettings(UpdateFieldTypePayload payload, FieldType fieldType, ActorId? actorId)
  {
    if (payload.Boolean != null)
    {
      BooleanSettings settings = new(payload.Boolean);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.DateTime != null)
    {
      DateTimeSettings settings = new(payload.DateTime);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Number != null)
    {
      NumberSettings settings = new(payload.Number);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.RelatedContent != null)
    {
      RelatedContentSettings settings = payload.RelatedContent.ToRelatedContentSettings(); // TODO(fpion): ensure ContentType exists
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.RichText != null)
    {
      RichTextSettings settings = new(payload.RichText);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Select != null)
    {
      SelectSettings settings = payload.Select.ToSelectSettings();
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.String != null)
    {
      StringSettings settings = new(payload.String);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Tags != null)
    {
      TagsSettings settings = new(payload.Tags);
      fieldType.SetSettings(settings, actorId);
    }
  }
}

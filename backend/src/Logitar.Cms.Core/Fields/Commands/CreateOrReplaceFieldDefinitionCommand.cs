using FluentValidation;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record CreateOrReplaceFieldDefinitionCommand(Guid ContentTypeId, Guid? FieldId, CreateOrReplaceFieldDefinitionPayload Payload) : IRequest<ContentTypeModel?>;

internal class CreateOrReplaceFieldDefinitionCommandHandler : IRequestHandler<CreateOrReplaceFieldDefinitionCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateOrReplaceFieldDefinitionCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeRepository fieldTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<ContentTypeModel?> Handle(CreateOrReplaceFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldDefinitionPayload payload = command.Payload;
    new CreateOrReplaceFieldDefinitionValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }
    else if (contentType.IsInvariant && !payload.IsInvariant)
    {
      throw new NotImplementedException(); // TODO(fpion): typed exception
    }

    FieldTypeId? fieldTypeId = command.FieldId.HasValue ? contentType.TryGetField(command.FieldId.Value)?.FieldTypeId : null;
    if (fieldTypeId == null)
    {
      if (!payload.FieldTypeId.HasValue)
      {
        throw new NotImplementedException(); // TODO(fpion): typed exception
      }

      fieldTypeId = new(payload.FieldTypeId.Value);
      _ = await _fieldTypeRepository.LoadAsync(fieldTypeId.Value, cancellationToken) ?? throw new FieldTypeNotFoundException(fieldTypeId.Value, nameof(payload.FieldTypeId));
    }

    Identifier uniqueName = new(payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    Placeholder? placeholder = Placeholder.TryCreate(payload.Placeholder);
    FieldDefinition fieldDefinition = new(fieldTypeId.Value, payload.IsInvariant, payload.IsRequired, payload.IsIndexed, payload.IsUnique, uniqueName, displayName, description, placeholder);

    ActorId? actorId = _applicationContext.ActorId;
    if (command.FieldId.HasValue)
    {
      contentType.SetField(command.FieldId.Value, fieldDefinition, actorId);
    }
    else
    {
      contentType.AddField(fieldDefinition, actorId);
    }

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

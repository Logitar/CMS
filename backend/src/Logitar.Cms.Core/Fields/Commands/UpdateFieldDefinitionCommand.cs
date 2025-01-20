using FluentValidation;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Validators;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Fields.Commands;

public record UpdateFieldDefinitionCommand(Guid ContentTypeId, Guid FieldId, UpdateFieldDefinitionPayload Payload) : IRequest<ContentTypeModel?>;

internal class UpdateFieldDefinitionCommandHandler : IRequestHandler<UpdateFieldDefinitionCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public UpdateFieldDefinitionCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository)
  {
    _applicationContext = applicationContext;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<ContentTypeModel?> Handle(UpdateFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    UpdateFieldDefinitionPayload payload = command.Payload;
    new UpdateFieldDefinitionValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    FieldDefinition? fieldDefinition = contentType?.TryGetField(command.FieldId);
    if (contentType == null || fieldDefinition == null)
    {
      return null;
    }

    fieldDefinition = new(
      fieldDefinition.Id,
      fieldDefinition.FieldTypeId,
      payload.IsInvariant ?? fieldDefinition.IsInvariant,
      payload.IsRequired ?? fieldDefinition.IsRequired,
      payload.IsIndexed ?? fieldDefinition.IsIndexed,
      payload.IsUnique ?? fieldDefinition.IsUnique,
      string.IsNullOrWhiteSpace(payload.UniqueName) ? fieldDefinition.UniqueName : new Identifier(payload.UniqueName),
      payload.DisplayName == null ? fieldDefinition.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value),
      payload.Description == null ? fieldDefinition.Description : Description.TryCreate(payload.Description.Value),
      payload.Placeholder == null ? fieldDefinition.Placeholder : Placeholder.TryCreate(payload.Placeholder.Value));
    contentType.SetField(fieldDefinition, _applicationContext.ActorId);

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

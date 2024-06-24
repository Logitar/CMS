using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class UpdateFieldDefinitionCommandHandler : IRequestHandler<UpdateFieldDefinitionCommand, ContentsType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public UpdateFieldDefinitionCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<ContentsType?> Handle(UpdateFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    UpdateFieldDefinitionPayload payload = command.Payload;
    new UpdateFieldDefinitionValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentTypeAggregate? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    FieldDefinitionUnit fieldDefinition = contentType.TryGetFieldDefinition(command.FieldId)
      ?? throw new FieldDefinitionNotFoundException(contentType, command.FieldId, nameof(command.FieldId));

    IdentifierUnit uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? fieldDefinition.UniqueName : new(payload.UniqueName);
    DisplayNameUnit? displayName = payload.DisplayName == null ? fieldDefinition.DisplayName : DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    DescriptionUnit? description = payload.Description == null ? fieldDefinition.Description : DescriptionUnit.TryCreate(payload.Description.Value);
    PlaceholderUnit? placeholder = payload.Placeholder == null ? fieldDefinition.Placeholder : PlaceholderUnit.TryCreate(payload.Placeholder.Value);

    fieldDefinition = new(fieldDefinition.FieldTypeId, uniqueName, displayName, description, placeholder,
      fieldDefinition.IsInvariant, fieldDefinition.IsRequired, fieldDefinition.IsIndexed, fieldDefinition.IsUnique);

    contentType.SetFieldDefinition(command.FieldId, fieldDefinition, command.ActorId);

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class ReplaceFieldDefinitionCommandHandler : IRequestHandler<ReplaceFieldDefinitionCommand, ContentsType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;

  public ReplaceFieldDefinitionCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
  }

  public async Task<ContentsType?> Handle(ReplaceFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    ReplaceFieldDefinitionPayload payload = command.Payload;
    new ReplaceFieldDefinitionValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentTypeAggregate? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    FieldDefinitionUnit fieldDefinition = contentType.TryGetFieldDefinition(command.FieldId)
      ?? throw new FieldDefinitionNotFoundException(contentType, command.FieldId, nameof(command.FieldId));

    IdentifierUnit uniqueName = new(payload.UniqueName);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    PlaceholderUnit? placeholder = PlaceholderUnit.TryCreate(payload.Placeholder);

    if (command.Version.HasValue)
    {
      ContentTypeAggregate? referenceContentType = await _contentTypeRepository.LoadAsync(contentType.Id, command.Version.Value, cancellationToken);
      if (referenceContentType != null)
      {
        FieldDefinitionUnit? reference = referenceContentType?.TryGetFieldDefinition(command.FieldId);
        if (reference != null)
        {
          if (uniqueName == reference.UniqueName)
          {
            uniqueName = fieldDefinition.UniqueName;
          }
          if (displayName == reference.DisplayName)
          {
            displayName = fieldDefinition.DisplayName;
          }
          if (description == reference.Description)
          {
            description = fieldDefinition.Description;
          }
          if (placeholder == reference.Placeholder)
          {
            placeholder = fieldDefinition.Placeholder;
          }
        }
      }
    }

    fieldDefinition = new(fieldDefinition.FieldTypeId, uniqueName, displayName, description, placeholder,
      fieldDefinition.IsInvariant, fieldDefinition.IsRequired, fieldDefinition.IsIndexed, fieldDefinition.IsUnique);

    contentType.SetFieldDefinition(command.FieldId, fieldDefinition, command.ActorId);

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Cms.Core.FieldTypes;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class CreateFieldDefinitionCommandHandler : IRequestHandler<CreateFieldDefinitionCommand, CmsContentType>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateFieldDefinitionCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository, IFieldTypeRepository fieldTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<CmsContentType> Handle(CreateFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentTypeAggregate contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<ContentTypeAggregate>(contentTypeId.AggregateId, nameof(command.ContentTypeId));

    CreateFieldDefinitionPayload payload = command.Payload;
    new CreateFieldDefinitionValidator(contentType.IsInvariant).ValidateAndThrow(payload);

    FieldTypeId fieldTypeId = new(payload.FieldTypeId);
    FieldTypeAggregate fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<FieldTypeAggregate>(fieldTypeId.AggregateId, nameof(payload.FieldTypeId));

    IdentifierUnit uniqueName = new(payload.UniqueName);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    PlaceholderUnit? placeholder = PlaceholderUnit.TryCreate(payload.Placeholder);
    FieldDefinitionUnit fieldDefinition = new(fieldType.Id, payload.IsInvariant, payload.IsRequired, payload.IsIndexed, payload.IsUnique,
      uniqueName, displayName, description, placeholder);
    contentType.AddFieldDefinition(fieldDefinition, command.ActorId);
    // CmsUniqueNameAlreadyUsedException

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

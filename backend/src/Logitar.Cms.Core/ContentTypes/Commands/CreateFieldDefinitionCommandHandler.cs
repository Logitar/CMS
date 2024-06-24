using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Cms.Core.Fields;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class CreateFieldDefinitionCommandHandler : IRequestHandler<CreateFieldDefinitionCommand, ContentsType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public CreateFieldDefinitionCommandHandler(IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository, IFieldTypeRepository fieldTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task<ContentsType?> Handle(CreateFieldDefinitionCommand command, CancellationToken cancellationToken)
  {
    CreateFieldDefinitionPayload payload = command.Payload;
    new CreateFieldDefinitionValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.ContentTypeId);
    ContentTypeAggregate? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    FieldTypeId fieldTypeId = new(payload.FieldTypeId);
    FieldTypeAggregate fieldType = await _fieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<FieldTypeAggregate>(fieldTypeId.AggregateId, nameof(payload.FieldTypeId));

    IdentifierUnit uniqueName = new(payload.UniqueName);
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    PlaceholderUnit? placeholder = PlaceholderUnit.TryCreate(payload.Placeholder);
    FieldDefinitionUnit fieldDefinition = new(fieldType.Id, uniqueName, displayName, description, placeholder,
      payload.IsInvariant, payload.IsRequired, payload.IsInvariant, payload.IsUnique);
    contentType.AddFieldDefinition(fieldDefinition, command.ActorId);

    await _contentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

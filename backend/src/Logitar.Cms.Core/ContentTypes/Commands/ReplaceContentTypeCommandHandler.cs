using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class ReplaceContentTypeCommandHandler : IRequestHandler<ReplaceContentTypeCommand, ContentsType?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ISender _sender;

  public ReplaceContentTypeCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository, ISender sender)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _sender = sender;
  }

  public async Task<ContentsType?> Handle(ReplaceContentTypeCommand command, CancellationToken cancellationToken)
  {
    ReplaceContentTypePayload payload = command.Payload;
    new ReplaceContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId id = new(command.Id);
    ContentTypeAggregate? contentType = await _contentTypeRepository.LoadAsync(id, cancellationToken);
    if (contentType == null)
    {
      return null;
    }
    ContentTypeAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _contentTypeRepository.LoadAsync(contentType.Id, command.Version.Value, cancellationToken);
    }

    IdentifierUnit uniqueName = new(payload.UniqueName);
    if (reference == null || uniqueName != reference.UniqueName)
    {
      contentType.UniqueName = uniqueName;
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      contentType.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      contentType.Description = description;
    }

    contentType.Update(command.ActorId);

    await _sender.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

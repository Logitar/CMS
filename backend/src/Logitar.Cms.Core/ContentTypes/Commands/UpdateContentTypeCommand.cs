using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record UpdateContentTypeCommand(Guid Id, UpdateContentTypePayload Payload) : Activity, IRequest<ContentTypeModel?>;

public class UpdateContentTypeCommandHandler : IRequestHandler<UpdateContentTypeCommand, ContentTypeModel?>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ISender _sender;

  public UpdateContentTypeCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository, ISender sender)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _sender = sender;
  }

  public async Task<ContentTypeModel?> Handle(UpdateContentTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateContentTypePayload payload = command.Payload;
    new UpdateContentTypeValidator().ValidateAndThrow(command.Payload);

    ContentTypeId contentTypeId = new(command.Id);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      contentType.UniqueName = new Identifier(payload.UniqueName);
    }
    if (payload.DisplayName != null)
    {
      contentType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      contentType.Description = Description.TryCreate(payload.Description.Value);
    }

    ActorId actorId = command.GetActorId();
    contentType.Update(actorId);

    await _sender.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

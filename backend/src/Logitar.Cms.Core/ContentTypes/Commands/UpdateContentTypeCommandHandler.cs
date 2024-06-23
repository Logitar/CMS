using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class UpdateContentTypeCommandHandler : IRequestHandler<UpdateContentTypeCommand, ContentsType?>
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

  public async Task<ContentsType?> Handle(UpdateContentTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateContentTypePayload payload = command.Payload;
    new UpdateContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId id = new(command.Id);
    ContentTypeAggregate? contentType = await _contentTypeRepository.LoadAsync(id, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      contentType.UniqueName = new IdentifierUnit(payload.UniqueName);
    }
    if (payload.DisplayName != null)
    {
      contentType.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      contentType.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    contentType.Update(command.ActorId);

    await _sender.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

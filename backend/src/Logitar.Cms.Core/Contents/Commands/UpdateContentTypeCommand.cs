using FluentValidation;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record UpdateContentTypeCommand(Guid Id, UpdateContentTypePayload Payload) : IRequest<ContentTypeModel?>;

internal class UpdateContentTypeCommandHandler : IRequestHandler<UpdateContentTypeCommand, ContentTypeModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IMediator _mediator;

  public UpdateContentTypeCommandHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _mediator = mediator;
  }

  public async Task<ContentTypeModel?> Handle(UpdateContentTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateContentTypePayload payload = command.Payload;
    new UpdateContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.Id);
    ContentType? contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType == null)
    {
      return null;
    }

    ActorId? actorId = _applicationContext.ActorId;

    if (payload.IsInvariant.HasValue)
    {
      contentType.IsInvariant = payload.IsInvariant.Value;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      Identifier uniqueName = new(payload.UniqueName);
      contentType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName != null)
    {
      contentType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      contentType.Description = Description.TryCreate(payload.Description.Value);
    }

    contentType.Update(actorId);

    await _mediator.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

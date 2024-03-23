using FluentValidation;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.Contents.Validators;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, ContentItem>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ISender _sender;

  public CreateContentCommandHandler(IContentQuerier contentQuerier, IContentTypeRepository contentTypeRepository, ISender sender)
  {
    _contentQuerier = contentQuerier;
    _contentTypeRepository = contentTypeRepository;
    _sender = sender;
  }

  public async Task<ContentItem> Handle(CreateContentCommand command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ContentAggregate.UniqueNameSettings;

    CreateContentPayload payload = command.Payload;
    new CreateContentValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ContentTypeAggregate contentType = await _contentTypeRepository.LoadAsync(payload.ContentTypeId, cancellationToken)
      ?? throw new AggregateNotFoundException<ContentTypeAggregate>(payload.ContentTypeId, nameof(payload.ContentTypeId));

    ContentAggregate content = new(contentType, new UniqueNameUnit(uniqueNameSettings, payload.UniqueName), command.ActorId);
    // TODO(fpion): DisplayName
    // TODO(fpion): Description

    await _sender.Send(new SaveContentCommand(content), cancellationToken);

    return await _contentQuerier.ReadAsync(content, cancellationToken);
  }
}

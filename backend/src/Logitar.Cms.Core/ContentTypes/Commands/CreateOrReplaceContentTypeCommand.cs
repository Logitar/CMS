using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

public record CreateOrReplaceContentTypeResult(ContentTypeModel? ContentType = null, bool Created = false);

public record CreateOrReplaceContentTypeCommand(Guid? Id, CreateOrReplaceContentTypePayload Payload, long? Version) : Activity, IRequest<CreateOrReplaceContentTypeResult>;

public class CreateOrReplaceContentTypeCommandHandler : IRequestHandler<CreateOrReplaceContentTypeCommand, CreateOrReplaceContentTypeResult>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ISender _sender;

  public CreateOrReplaceContentTypeCommandHandler(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository, ISender sender)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _contentTypeRepository = contentTypeRepository;
    _sender = sender;
  }

  public async Task<CreateOrReplaceContentTypeResult> Handle(CreateOrReplaceContentTypeCommand command, CancellationToken cancellationToken)
  {
    new CreateOrReplaceContentTypeValidator().ValidateAndThrow(command.Payload);

    bool created = false;
    ContentType? contentType = await FindAsync(command, cancellationToken);
    if (contentType == null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceContentTypeResult();
      }

      contentType = Create(command);
      created = true;
    }
    else
    {
      await ReplaceAsync(command, contentType, cancellationToken);
    }

    await _sender.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    ContentTypeModel model = await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
    return new CreateOrReplaceContentTypeResult(model, created);
  }

  private async Task<ContentType?> FindAsync(CreateOrReplaceContentTypeCommand command, CancellationToken cancellationToken)
  {
    if (command.Id == null)
    {
      return null;
    }

    ContentTypeId contentTypeId = new(command.Id.Value);
    return await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
  }

  private static ContentType Create(CreateOrReplaceContentTypeCommand command)
  {
    CreateOrReplaceContentTypePayload payload = command.Payload;
    ActorId actorId = command.GetActorId();

    Identifier uniqueName = new(payload.UniqueName);
    ContentTypeId? contentTypeId = command.Id.HasValue ? new(command.Id.Value) : null;
    ContentType contentType = new(uniqueName, payload.IsInvariant, actorId, contentTypeId)
    {
      DisplayName = DisplayName.TryCreate(payload.DisplayName),
      Description = Description.TryCreate(payload.Description)
    };

    contentType.Update(actorId);

    return contentType;
  }

  private async Task ReplaceAsync(CreateOrReplaceContentTypeCommand command, ContentType contentType, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypePayload payload = command.Payload;
    ActorId actorId = command.GetActorId();

    ContentType? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _contentTypeRepository.LoadAsync(contentType.Id, command.Version.Value, cancellationToken);
    }
    reference ??= contentType;

    Identifier uniqueName = new(payload.UniqueName);
    if (reference.UniqueName != uniqueName)
    {
      contentType.UniqueName = uniqueName;
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      contentType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      contentType.Description = description;
    }

    contentType.Update(actorId);
  }
}

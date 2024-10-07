using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.Core.ContentTypes.Validators;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.ContentTypes.Commands;

internal class CreateContentTypeCommandHandler : IRequestHandler<CreateContentTypeCommand, CmsContentType>
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly ISender _sender;

  public CreateContentTypeCommandHandler(IContentTypeQuerier contentTypeQuerier, ISender sender)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _sender = sender;
  }

  public async Task<CmsContentType> Handle(CreateContentTypeCommand command, CancellationToken cancellationToken)
  {
    CreateContentTypePayload payload = command.Payload;
    new CreateContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeAggregate contentType = new(new IdentifierUnit(payload.UniqueName), payload.IsInvariant, command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    contentType.Update(command.ActorId);

    await _sender.Send(new SaveContentTypeCommand(contentType), cancellationToken);

    return await _contentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}

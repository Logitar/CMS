using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentCommand(Content Content) : IRequest;

internal class SaveContentCommandHandler : IRequestHandler<SaveContentCommand>
{
  private readonly IContentRepository _contentRepository;

  public SaveContentCommandHandler(IContentRepository contentRepository)
  {
    _contentRepository = contentRepository;
  }

  public async Task Handle(SaveContentCommand command, CancellationToken cancellationToken)
  {
    Content content = command.Content;

    // TODO(fpion): ensure UniqueName is not taken within ContentType

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}

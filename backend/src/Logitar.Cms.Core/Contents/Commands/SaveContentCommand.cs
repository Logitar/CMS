using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal record SaveContentCommand(Content Content) : IRequest;

internal class SaveContentCommandHandler : IRequestHandler<SaveContentCommand>
{
  private readonly IContentQuerier _contentQuerier;
  private readonly IContentRepository _contentRepository;

  public SaveContentCommandHandler(IContentQuerier contentQuerier, IContentRepository contentRepository)
  {
    _contentQuerier = contentQuerier;
    _contentRepository = contentRepository;
  }

  public async Task Handle(SaveContentCommand command, CancellationToken cancellationToken)
  {
    Content content = command.Content;

    // TODO(fpion): ensure unique names are not already used

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}

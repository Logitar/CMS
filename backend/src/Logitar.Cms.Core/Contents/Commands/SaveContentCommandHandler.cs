using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

internal class SaveContentCommandHandler : IRequestHandler<SaveContentCommand>
{
  private readonly IContentRepository _contentRepository;

  public SaveContentCommandHandler(IContentRepository contentRepository)
  {
    _contentRepository = contentRepository;
  }

  public async Task Handle(SaveContentCommand command, CancellationToken cancellationToken)
  {
    ContentAggregate content = command.Content;

    // TODO(fpion): ensure UniqueName unicity per ContentType & Language

    await _contentRepository.SaveAsync(content, cancellationToken);
  }
}

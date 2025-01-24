using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class PublishContentCommandHandlerTests
{
  //private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();

  private readonly PublishContentCommandHandler _handler;

  public PublishContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object);
  }

  // TODO(fpion): implement
}

using Moq;

namespace Logitar.Cms.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadApiKeyQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();

  private readonly ReadApiKeyQueryHandler _handler;

  public ReadApiKeyQueryHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when no API key is found.")]
  public async Task It_should_return_null_when_no_Api_key_is_found()
  {
    ReadApiKeyQuery query = new(Guid.Parse("a783dacf-894b-4e30-9983-b57b509b9395"));
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }
}

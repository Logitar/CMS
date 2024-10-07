using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadApiKeyQueryHandlerTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;

  private readonly ApiKeyAggregate _apiKey;

  public ReadApiKeyQueryHandlerTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();

    _apiKey = new(new DisplayNameUnit("Test API Key"), new PasswordMock("Test123!"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _apiKeyRepository.SaveAsync(_apiKey);
  }

  [Fact(DisplayName = "It should return the API key found by ID.")]
  public async Task It_should_return_the_Api_key_found_by_Id()
  {
    ReadApiKeyQuery query = new(_apiKey.Id.ToGuid());
    ApiKey? apiKey = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.Id.ToGuid(), apiKey.Id);
  }
}

using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.ApiKeys;

namespace Logitar.Cms.Core.ApiKeys.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateApiKeyCommandHandlerTests : IntegrationTests
{
  public CreateApiKeyCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new API key.")]
  public async Task It_should_create_a_new_Api_key()
  {
    CreateApiKeyPayload payload = new(" Test API Key ")
    {
      Description = "  This is a test API key.  ",
      ExpiresOn = DateTime.Now.AddDays(1)
    };
    payload.CustomAttributes.Add(new CustomAttribute("UserId", "a12f7902-62fe-423b-886c-bf5539ff0994"));
    CreateApiKeyCommand command = new(payload);

    ApiKey apiKey = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, apiKey.Id);
    Assert.Equal(2, apiKey.Version);
    Assert.NotEqual(default, apiKey.CreatedOn);
    Assert.True(apiKey.CreatedOn < apiKey.UpdatedOn);
    Assert.Equal(Actor, apiKey.CreatedBy);
    Assert.Equal(Actor, apiKey.UpdatedBy);

    Assert.NotNull(apiKey.XApiKey);

    Assert.Equal(payload.DisplayName.Trim(), apiKey.DisplayName);
    Assert.Equal(payload.Description.Trim(), apiKey.Description);
    Assert.Equal(payload.ExpiresOn?.AsUniversalTime(), apiKey.ExpiresOn);
    Assert.Null(apiKey.AuthenticatedOn);
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);
    Assert.Empty(apiKey.Roles);
  }
}

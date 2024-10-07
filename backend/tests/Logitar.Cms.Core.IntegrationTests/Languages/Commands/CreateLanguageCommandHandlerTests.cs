using Logitar.Cms.Contracts.Languages;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateLanguageCommandHandlerTests : IntegrationTests
{
  public CreateLanguageCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new language.")]
  public async Task It_should_create_a_new_language()
  {
    CreateLanguagePayload payload = new("fr");
    CreateLanguageCommand command = new(payload);
    Language language = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, language.Id);
    Assert.Equal(1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.NotEqual(default, language.CreatedOn);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(language.CreatedOn, language.UpdatedOn);

    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}

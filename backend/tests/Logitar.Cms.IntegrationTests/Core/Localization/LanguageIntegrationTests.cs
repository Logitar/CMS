using Logitar.Cms.Core.Localization.Commands;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Infrastructure;

namespace Logitar.Cms.Core.Localization;

[Trait(Traits.Category, Categories.Integration)]
public class LanguageIntegrationTests : IntegrationTests
{
  public LanguageIntegrationTests() : base(DatabaseProvider.SqlServer)
  {
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(null)]
  [InlineData("36393a67-548d-4366-be96-71dddc7a8327")]
  public async Task Given_NotExist_When_Create_Then_LanguageCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceLanguagePayload payload = new("fr");
    CreateOrReplaceLanguageCommand command = new(id, payload, Version: null);
    CreateOrReplaceLanguageResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    LanguageModel? language = result.Language;
    Assert.NotNull(language);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, language.Id);
    }
    Assert.Equal(1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromMinutes(1));
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}

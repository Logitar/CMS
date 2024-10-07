using Logitar.Cms.Contracts.Languages;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Languages.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SetDefaultLanguageCommandHandlerTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  public SetDefaultLanguageCommandHandlerTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task It_should_set_the_default_language()
  {
    LanguageAggregate language = new(new LocaleUnit("fr"), isDefault: false, ActorId);
    await _languageRepository.SaveAsync(language);

    SetDefaultLanguageCommand command = new(language.Id.ToGuid());
    Language? result = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(result);

    Assert.Equal(language.Id.ToGuid(), result.Id);
    Assert.Equal(language.Version + 1, result.Version);
    Assert.Equal(language.CreatedBy.ToGuid(), result.CreatedBy.Id);
    Assert.Equal(language.CreatedOn.AsUniversalTime(), result.CreatedOn);
    Assert.Equal(language.UpdatedBy.ToGuid(), result.UpdatedBy.Id);
    Assert.True(result.CreatedOn < result.UpdatedOn);

    Assert.True(result.IsDefault);

    IReadOnlyCollection<LanguageAggregate> languages = await _languageRepository.LoadAsync();
    foreach (LanguageAggregate aggregate in languages)
    {
      Assert.Equal(language.Equals(aggregate), aggregate.IsDefault);
    }
  }
}

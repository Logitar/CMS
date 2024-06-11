using Logitar.Cms.Contracts.Localization;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SetDefaultLanguageCommandTests : IntegrationTests
{
  private readonly ILanguageRepository _languageRepository;

  private readonly LanguageAggregate _language;

  public SetDefaultLanguageCommandTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _language = new(new LocaleUnit("fr"), isDefault: false, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_language);
  }

  [Fact(DisplayName = "It should not change anything when the language is already default.")]
  public async Task It_should_not_change_anything_when_the_language_is_already_default()
  {
    LanguageAggregate @default = await _languageRepository.LoadDefaultAsync();

    SetDefaultLanguageCommand command = new(@default.Id.ToGuid());
    Language? result = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(@default.Id.ToGuid(), result.Id);
    Assert.Equal(@default.Version, result.Version);
    Assert.True(result.IsDefault);

    LanguageAggregate? language = await _languageRepository.LoadAsync(_language.Id);
    Assert.NotNull(language);
    Assert.Equal(_language, language);
    Assert.Equal(_language.Version, language.Version);
    Assert.False(language.IsDefault);
  }

  [Fact(DisplayName = "It should return null when the language could not be found.")]
  public async Task It_should_return_null_when_the_language_could_not_be_found()
  {
    SetDefaultLanguageCommand command = new(Guid.NewGuid());
    Language? language = await Pipeline.ExecuteAsync(command);
    Assert.Null(language);
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task It_should_set_the_default_language()
  {
    LanguageAggregate @default = await _languageRepository.LoadDefaultAsync();

    SetDefaultLanguageCommand command = new(_language.Id.ToGuid());
    Language? language = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(language);
    Assert.Equal(_language.Id.ToGuid(), language.Id);
    Assert.Equal(_language.Version + 1, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.True(_language.UpdatedOn < language.UpdatedOn);
    Assert.True(language.IsDefault);

    LanguageAggregate? oldDefault = await _languageRepository.LoadAsync(@default.Id);
    Assert.NotNull(oldDefault);
    Assert.False(oldDefault.IsDeleted);
    Assert.Equal(oldDefault.UpdatedBy, ActorId);
    Assert.True(@default.UpdatedOn < oldDefault.UpdatedOn);
  }
}

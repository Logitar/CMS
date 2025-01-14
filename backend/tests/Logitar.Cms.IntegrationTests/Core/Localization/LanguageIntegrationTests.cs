using Logitar.Cms.Core.Localization.Commands;
using Logitar.Cms.Core.Localization.Models;
using Logitar.Cms.Core.Localization.Queries;
using Logitar.Cms.Core.Search;
using Logitar.Identity.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Localization;

[Trait(Traits.Category, Categories.Integration)]
public class LanguageIntegrationTests : IntegrationTests
{
  private readonly ILanguageManager _languageManager;
  private readonly ILanguageRepository _languageRepository;

  public LanguageIntegrationTests() : base()
  {
    _languageManager = ServiceProvider.GetRequiredService<ILanguageManager>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
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

  [Fact(DisplayName = "It should read a language.")]
  public async Task Given_Exists_When_Read_Then_LanguageReturned()
  {
    Language language = Assert.Single(await _languageRepository.LoadAsync());

    ReadLanguageQuery query = new(language.Id.ToGuid(), language.Locale.Code, IsDefault: true);
    LanguageModel? model = await Mediator.Send(query);

    Assert.NotNull(model);
    Assert.Equal(language.Id.ToGuid(), model.Id);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_ExistsNoVersion_When_Replace_Then_LanguageReplaced()
  {
    Language english = Assert.Single(await _languageRepository.LoadAsync());

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr"
    };
    CreateOrReplaceLanguageCommand command = new(english.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceLanguageResult result = await Mediator.Send(command);
    Assert.False(result.Created);

    LanguageModel? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(english.Version + 1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(english.IsDefault, language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }

  [Fact(DisplayName = "It should replace/update an existing language.")]
  public async Task Given_ExistsVersion_When_Replace_Then_LanguageUpdated()
  {
    Language english = Assert.Single(await _languageRepository.LoadAsync());
    long version = english.Version;
    english.SetLocale(new Locale("en-US"), ActorId);
    await _languageRepository.SaveAsync(english);

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "en"
    };
    CreateOrReplaceLanguageCommand command = new(english.Id.ToGuid(), payload, version);
    CreateOrReplaceLanguageResult result = await Mediator.Send(command);
    Assert.False(result.Created);

    LanguageModel? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(english.Version, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(english.IsDefault, language.IsDefault);
    Assert.Equal(english.Locale.Code, language.Locale.Code);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Parameters_When_Search_Then_CorrectResults()
  {
    Language english = Assert.Single(await _languageRepository.LoadAsync());
    Language french = new(new Locale("fr"));
    Language canadianEnglish = new(new Locale("en-CA"));
    Language canadianFrench = new(new Locale("fr-CA"));
    await _languageRepository.SaveAsync([french, canadianEnglish, canadianFrench]);

    SearchLanguagesPayload payload = new()
    {
      Ids = [english.Id.ToGuid(), french.Id.ToGuid(), canadianFrench.Id.ToGuid(), Guid.NewGuid()],
      Search = new TextSearch([new SearchTerm("%-CA"), new SearchTerm("%fr%")], SearchOperator.Or),
      Sort = [new LanguageSortOption(LanguageSort.Code, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchLanguagesQuery query = new(payload);
    SearchResults<LanguageModel> results = await Mediator.Send(query);

    Assert.Equal(2, results.Total);
    LanguageModel result = Assert.Single(results.Items);
    Assert.Equal(french.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task Given_Language_When_SetDefault_Then_DefaultSet()
  {
    LanguageId englishId = Assert.Single(await _languageRepository.LoadAsync()).Id;

    Language french = new(new Locale("fr"), isDefault: false, ActorId);
    await _languageRepository.SaveAsync(french);

    SetDefaultLanguageCommand command = new(french.Id.ToGuid());
    LanguageModel? language = await Mediator.Send(command);

    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(french.Version + 1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.True(language.IsDefault);
    Assert.Equal(french.Locale.Code, language.Locale.Code);

    Language? english = await _languageRepository.LoadAsync(englishId);
    Assert.NotNull(english);
    Assert.False(english.IsDefault);
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when a locale conflict occurs while saving a language.")]
  public async Task Given_LocaleConflict_When_Save_Then_LocaleAlreadyUsedException()
  {
    Language oldLanguage = Assert.Single(await _languageRepository.LoadAsync());
    Language newLanguage = new(new Locale("en"));

    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _languageManager.SaveAsync(newLanguage));
    Assert.Equal(oldLanguage.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(newLanguage.Id.ToGuid(), exception.LanguageId);
    Assert.Equal(newLanguage.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Language_When_Update_Then_Updated()
  {
    Language english = Assert.Single(await _languageRepository.LoadAsync());

    UpdateLanguagePayload payload = new()
    {
      Locale = "fr"
    };
    UpdateLanguageCommand command = new(english.Id.ToGuid(), payload);
    LanguageModel? language = await Mediator.Send(command);

    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(english.Version + 1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(english.IsDefault, language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}

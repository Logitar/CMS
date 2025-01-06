using Logitar.Cms.Core.Contents.Commands;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.Identity.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class ContentIntegrationTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly Language _french;
  private readonly ContentType _brand;
  private readonly ContentType _cymbal;

  public ContentIntegrationTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _french = new(new Locale("fr"), isDefault: false);

    _brand = new(new Identifier("Brand"), isInvariant: true);
    _cymbal = new(new Identifier("Cymbal"), isInvariant: false);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_french);
    await _contentTypeRepository.SaveAsync([_brand, _cymbal]);
  }

  [Fact(DisplayName = "It should create a new content locale.")]
  public async Task Given_NewLocale_When_Save_Then_ContentLocaleCreated()
  {
    Language? english = (await _languageRepository.LoadAsync()).SingleOrDefault(x => x.IsDefault);
    Assert.NotNull(english);

    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "Sabian-AA-17-Inch-Holy-China-Traditional-Finish"));
    Content cymbal = new(_cymbal, invariant, ActorId);
    ContentLocale locale = new(
      new UniqueName(Content.UniqueNameSettings, "Sabian-AA-17-Inch-Holy-China-Traditional-Finish"),
      new DisplayName(" Sabian AA 17 \" Holy China Traditional Finish "),
      new Description("  The 17\" Holy China delivers all of the unique, trashy tone of the 21\" Holy China, with less volume.\n\nSTYLE: VINTAGE\nMETAL: B20\nSOUND: BRIGHT\nWEIGHT: THIN  "));
    cymbal.SetLocale(english, locale, ActorId);
    await _contentRepository.SaveAsync(cymbal);

    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = "Sabian-AA-17-Inch-Holy-China-Traditional-Finish",
      DisplayName = " Sabian Cymbale AA Holy China finition traditionnelle de 17 pouces ",
      Description = "  Le 17\" Holy China offre tout le son unique et trash du 21\" Holy China, avec moins de volume.\n\nSTYLE : VINTAGE\nMÉTAL : B20\nSON : BRIGHT\nPOIDS : MINCE  "
    };
    CreateOrReplaceContentCommand command = new(cymbal.Id.ToGuid(), _french.Id.ToGuid(), payload, Version: 999);
    CreateOrReplaceContentResult result = await Mediator.Send(command);
    Assert.False(result.Created);

    ContentModel? content = result.Content;
    Assert.NotNull(content);

    Assert.Equal(command.ContentId, content.Id);
    Assert.Equal(cymbal.Version + 1, content.Version);
    Assert.Equal(Actor, content.CreatedBy);
    Assert.Equal(DateTime.UtcNow, content.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromMinutes(1));

    ContentLocaleModel invariantModel = content.Invariant;
    Assert.Same(content, invariantModel.Content);
    Assert.Null(invariantModel.Language);
    Assert.Equal(invariant.UniqueName.Value, invariantModel.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, invariantModel.DisplayName);
    Assert.Equal(invariant.Description?.Value, invariantModel.Description);
    Assert.Equal(Actor, invariantModel.CreatedBy);
    Assert.Equal(DateTime.UtcNow, invariantModel.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, invariantModel.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, invariantModel.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(2, content.Locales.Count);

    ContentLocaleModel englishLocale = Assert.Single(content.Locales, l => l.Language != null && l.Language.Id == english.Id.ToGuid());
    Assert.Same(content, englishLocale.Content);
    Assert.Equal(locale.UniqueName.Value, englishLocale.UniqueName);
    Assert.Equal(locale.DisplayName?.Value, englishLocale.DisplayName);
    Assert.Equal(locale.Description?.Value, englishLocale.Description);
    Assert.Equal(Actor, englishLocale.CreatedBy);
    Assert.Equal(DateTime.UtcNow, englishLocale.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, englishLocale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, englishLocale.UpdatedOn, TimeSpan.FromMinutes(1));

    ContentLocaleModel frenchLocale = Assert.Single(content.Locales, l => l.Language != null && l.Language.Id == _french.Id.ToGuid());
    Assert.Same(content, frenchLocale.Content);
    Assert.Equal(payload.UniqueName, frenchLocale.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), frenchLocale.DisplayName);
    Assert.Equal(payload.Description.Trim(), frenchLocale.Description);
    Assert.Equal(Actor, frenchLocale.CreatedBy);
    Assert.Equal(DateTime.UtcNow, frenchLocale.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, frenchLocale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, frenchLocale.UpdatedOn, TimeSpan.FromMinutes(1));
  }

  [Theory(DisplayName = "It should create a new invariant content.")]
  [InlineData(null)]
  [InlineData("d11ec43b-9db3-4487-90b4-f423bf8e4455")]
  public async Task Given_Invariant_When_Create_Then_ContentCreated(string? idValue)
  {
    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = _brand.Id.ToGuid(),
      UniqueName = "Sabian",
      DisplayName = " Sabian ",
      Description = "  Sabian is a Canadian cymbal manufacturing company based in New Brunswick. It was established in 1981 in the village of Meductic, which is now part of Lakeland Ridges, where the company is still headquartered. Sabian is considered one of the big four manufacturers of cymbals, along with Zildjian, Meinl and Paiste.  "
    };
    CreateOrReplaceContentCommand command = new(idValue == null ? null : Guid.Parse(idValue), LanguageId: null, payload, Version: null);
    CreateOrReplaceContentResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    ContentModel? content = result.Content;
    Assert.NotNull(content);

    if (command.ContentId.HasValue)
    {
      Assert.Equal(command.ContentId.Value, content.Id);
    }
    Assert.Equal(1, content.Version);
    Assert.Equal(Actor, content.CreatedBy);
    Assert.Equal(DateTime.UtcNow, content.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.ContentTypeId, content.ContentType.Id);

    ContentLocaleModel invariant = content.Invariant;
    Assert.Same(content, invariant.Content);
    Assert.Null(invariant.Language);
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), invariant.DisplayName);
    Assert.Equal(payload.Description.Trim(), invariant.Description);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Empty(content.Locales);
  }

  [Theory(DisplayName = "It should create a new localized content.")]
  [InlineData(null)]
  [InlineData("e0095ee0-8aec-4916-b206-38e843a8d54a")]
  public async Task Given_Localized_When_Create_Then_ContentCreated(string? idValue)
  {
    Language? english = (await _languageRepository.LoadAsync()).SingleOrDefault(x => x.IsDefault);
    Assert.NotNull(english);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = _cymbal.Id.ToGuid(),
      UniqueName = "Sabian-AA-17-Inch-Holy-China-Traditional-Finish",
      DisplayName = " Sabian AA 17 \" Holy China Traditional Finish ",
      Description = "  The 17\" Holy China delivers all of the unique, trashy tone of the 21\" Holy China, with less volume.\n\nSTYLE: VINTAGE\nMETAL: B20\nSOUND: BRIGHT\nWEIGHT: THIN  "
    };
    CreateOrReplaceContentCommand command = new(idValue == null ? null : Guid.Parse(idValue), english.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceContentResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    ContentModel? content = result.Content;
    Assert.NotNull(content);

    if (command.ContentId.HasValue)
    {
      Assert.Equal(command.ContentId.Value, content.Id);
    }
    Assert.Equal(2, content.Version);
    Assert.Equal(Actor, content.CreatedBy);
    Assert.Equal(DateTime.UtcNow, content.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.ContentTypeId, content.ContentType.Id);

    ContentLocaleModel invariant = content.Invariant;
    Assert.Same(content, invariant.Content);
    Assert.Null(invariant.Language);
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), invariant.DisplayName);
    Assert.Equal(payload.Description.Trim(), invariant.Description);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.UpdatedOn, TimeSpan.FromMinutes(1));

    ContentLocaleModel locale = Assert.Single(content.Locales);
    Assert.Same(content, locale.Content);
    Assert.Equal(command.LanguageId, locale.Language?.Id);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), locale.DisplayName);
    Assert.Equal(payload.Description.Trim(), locale.Description);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.UpdatedOn, TimeSpan.FromMinutes(1));
  }
}

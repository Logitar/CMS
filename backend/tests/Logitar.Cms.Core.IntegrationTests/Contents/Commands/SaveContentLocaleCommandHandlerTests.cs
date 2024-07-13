using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SaveContentLocaleCommandHandlerTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly ContentTypeAggregate _contentType;
  private readonly ContentAggregate _content;

  public SaveContentLocaleCommandHandlerTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _contentType = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
    _content = new(_contentType, new ContentLocaleUnit(new UniqueNameUnit(ContentAggregate.UniqueNameSettings, "rendered-lego-acura-models")), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_contentType);
    await _contentRepository.SaveAsync(_content);
  }

  [Fact(DisplayName = "It should save a content locale.")]
  public async Task It_should_save_a_content_locale()
  {
    LanguageAggregate language = Assert.Single(await _languageRepository.LoadAsync());

    SaveContentLocalePayload payload = new("rendered-lego-acura-models-2");
    SaveContentLocaleCommand command = new(_content.Id.ToGuid(), language.Id.ToGuid(), payload);
    ContentItem? content = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(content);

    Assert.Equal(_content.Id.ToGuid(), content.Id);
    Assert.Equal(_content.Version + 1, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.True(content.CreatedOn < content.UpdatedOn);

    ContentLocale locale = Assert.Single(content.Locales, l => l.Language != null);
    Assert.NotEqual(default, locale.Id);
    Assert.Same(content, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(language.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.NotEqual(default, locale.CreatedOn);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(locale.CreatedOn, locale.UpdatedOn);
  }
}

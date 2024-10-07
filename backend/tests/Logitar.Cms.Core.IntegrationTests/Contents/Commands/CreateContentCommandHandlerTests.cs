using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateContentCommandHandlerTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  private readonly ContentTypeAggregate _blogArticle;

  public CreateContentCommandHandlerTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _blogArticle = new(new IdentifierUnit("BlogArticle"), isInvariant: false);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_blogArticle);
  }

  [Fact(DisplayName = "It should create a new content item and its locales.")]
  public async Task It_should_create_a_new_content_item_and_its_locales()
  {
    LanguageAggregate english = Assert.Single(await _languageRepository.LoadAsync());

    CreateContentPayload payload = new("rendered-lego-acura-models")
    {
      ContentTypeId = _blogArticle.Id.ToGuid(),
      LanguageId = english.Id.ToGuid()
    };
    CreateContentCommand command = new(payload);
    ContentItem item = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, item.Id);
    Assert.Equal(2, item.Version);
    Assert.Equal(Actor, item.CreatedBy);
    Assert.NotEqual(default, item.CreatedOn);
    Assert.Equal(Actor, item.UpdatedBy);
    Assert.True(item.CreatedOn < item.UpdatedOn);

    Assert.Equal(_blogArticle.Id.ToGuid(), item.ContentType.Id);

    ContentLocale invariant = item.Invariant;
    Assert.NotEqual(default, invariant.Id);
    Assert.Same(item, invariant.Item);
    Assert.Null(invariant.Language);
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.NotEqual(default, invariant.CreatedOn);
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(invariant.CreatedOn, invariant.UpdatedOn);

    ContentLocale locale = Assert.Single(item.Locales);
    Assert.NotEqual(default, locale.Id);
    Assert.Same(item, locale.Item);
    Assert.NotNull(locale.Language);
    Assert.Equal(english.Id.ToGuid(), locale.Language.Id);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.NotEqual(default, locale.CreatedOn);
    Assert.Equal(Actor, locale.UpdatedBy);
    Assert.Equal(locale.CreatedOn, locale.UpdatedOn);
  }
}

﻿using Bogus;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UnpublishContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();

  private readonly UnpublishContentCommandHandler _handler;

  private readonly Language _language = new(new Locale("en"), isDefault: true);
  private readonly ContentType _contentType = new(new Identifier("BlogArticle"), isInvariant: false);
  private readonly Content _content;
  private readonly ContentModel _model = new();

  public UnpublishContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);

    _content = new(_contentType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, "my-blog-article")));
    _content.SetLocale(_language, _content.Invariant);
    _content.PublishInvariant();
    _content.PublishLocale(_language);
    _contentQuerier.Setup(x => x.ReadAsync(_content, _cancellationToken)).ReturnsAsync(_model);
    _contentRepository.Setup(x => x.LoadAsync(_content.Id, _cancellationToken)).ReturnsAsync(_content);
  }

  [Fact(DisplayName = "It should unpublish a locale.")]
  public async Task Given_Locale_When_Handle_Then_LocaleUnpublished()
  {
    UnpublishContentCommand command = new(_content.Id.ToGuid(), _language.Id.ToGuid());
    ContentModel? model = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(model);
    Assert.Same(_model, model);

    _contentManager.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once);

    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished
      && unpublished.LanguageId == _language.Id && unpublished.ActorId == _actorId);
  }

  [Fact(DisplayName = "It should unpublish content invariant and all locales.")]
  public async Task Given_Content_When_Handle_Then_InvariantAndLocalesUnpublished()
  {
    UnpublishContentCommand command = new(_content.Id.ToGuid());
    ContentModel? model = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(model);
    Assert.Same(_model, model);

    _contentManager.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once);

    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished
      && unpublished.LanguageId == null && unpublished.ActorId == _actorId);
    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished
      && unpublished.LanguageId == _language.Id && unpublished.ActorId == _actorId);
  }

  [Fact(DisplayName = "It should unpublish the invariant.")]
  public async Task Given_Invariant_When_Handle_Then_InvariantUnpublished()
  {
    UnpublishContentCommand command = new(_content.Id.ToGuid(), languageId: null);
    ContentModel? model = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(model);
    Assert.Same(_model, model);

    _contentManager.Verify(x => x.SaveAsync(_content, _cancellationToken), Times.Once);

    Assert.Contains(_content.Changes, change => change is ContentLocaleUnpublished unpublished
      && unpublished.LanguageId == null && unpublished.ActorId == _actorId);
  }

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task Given_ContentNotFound_When_Handle_Then_NullReturned()
  {
    UnpublishContentCommand command = new(Guid.NewGuid());
    ContentModel? content = await _handler.Handle(command, _cancellationToken);
    Assert.Null(content);
  }

  [Fact(DisplayName = "It should return null when the locale could not be found.")]
  public async Task Given_LocaleNotFound_When_Handle_Then_NullReturned()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"));
    Content content = new(contentType, new ContentLocale(new UniqueName(Content.UniqueNameSettings, _faker.Person.UserName)));
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);

    UnpublishContentCommand command = new(content.Id.ToGuid(), Guid.NewGuid());
    ContentModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.Null(result);
  }
}

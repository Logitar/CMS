using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Cms.Core.Contents.Commands;

public class CreateOrReplaceContentCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentManager> _contentManager = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IContentTypeRepository> _contentTypeRepository = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly CreateOrReplaceContentCommandHandler _handler;

  public CreateOrReplaceContentCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentManager.Object, _contentQuerier.Object, _contentRepository.Object, _contentTypeRepository.Object, _languageRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should throw ContentTypeNotFoundException when the content type could not be found.")]
  public async Task Given_ContentTypeNotFound_When_Handle_Then_ContentTypeNotFoundException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = Guid.NewGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ContentTypeNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(payload.ContentTypeId.Value, exception.ContentTypeId);
    Assert.Equal("ContentTypeId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when creating content and the language could not be found.")]
  public async Task Given_NotExistsLanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.ToGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw LanguageNotFoundException when replacing content and the language could not be found.")]
  public async Task Given_ExistsLanguageNotFound_When_Handle_Then_LanguageNotFoundException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.ToGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(content.Id.ToGuid(), LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<LanguageNotFoundException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.True(command.LanguageId.HasValue);
    Assert.Equal(command.LanguageId.Value, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating content and the content type ID is null.")]
  public async Task Given_NotExistsContentTypeIdNull_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("RequiredValidator", error.ErrorCode);
    Assert.Equal("ContentTypeId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating invariant content in a variant content type.")]
  public async Task Given_VariantWithoutLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.ToGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' cannot be null. The content type is not invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when creating variant content in an invariant content type.")]
  public async Task Given_NotExistsInvariantWithLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    _contentTypeRepository.Setup(x => x.LoadAsync(contentType.Id, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.ToGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' must be null. The content type is invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when replacing variant content in an invariant content type.")]
  public async Task Given_ExistsInvariantWithLanguageId_When_Handle_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true);
    ContentLocale invariant = new(new UniqueName(Content.UniqueNameSettings, "my-blog-article"));
    Content content = new(contentType, invariant);
    _contentRepository.Setup(x => x.LoadAsync(content.Id, _cancellationToken)).ReturnsAsync(content);
    _contentTypeRepository.Setup(x => x.LoadAsync(content, _cancellationToken)).ReturnsAsync(contentType);

    CreateOrReplaceContentPayload payload = new()
    {
      ContentTypeId = contentType.Id.ToGuid(),
      UniqueName = new("my-blog-article")
    };
    CreateOrReplaceContentCommand command = new(content.Id.ToGuid(), LanguageId: Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("InvariantValidator", error.ErrorCode);
    Assert.Equal("'LanguageId' must be null. The content type is invariant.", error.ErrorMessage);
    Assert.Equal("LanguageId", error.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceContentPayload payload = new()
    {
      UniqueName = new("MyBlogArticle!"),
      DisplayName = RandomStringGenerator.GetString(999),
      FieldValues = [new FieldValue(Guid.Empty, string.Empty)]
    };
    CreateOrReplaceContentCommand command = new(ContentId: null, LanguageId: null, payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "FieldValues[0].Value");
  }
}

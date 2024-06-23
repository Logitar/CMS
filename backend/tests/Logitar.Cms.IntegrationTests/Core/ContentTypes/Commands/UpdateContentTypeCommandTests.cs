using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateContentTypeCommandTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _contentType;

  public UpdateContentTypeCommandTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _contentType = new(new IdentifierUnit("BlogArticle"), isInvariant: false, ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync(_contentType);
  }

  [Fact(DisplayName = "It should return null when the content type could not be found.")]
  public async Task It_should_return_null_when_the_content_type_could_not_be_found()
  {
    UpdateContentTypePayload payload = new();
    UpdateContentTypeCommand command = new(Id: Guid.NewGuid(), payload);
    Assert.Null(await Pipeline.ExecuteAsync(command));
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentTypeAggregate contentType = new(new IdentifierUnit("BlogCategory"), isInvariant: false, ActorId);
    await _contentTypeRepository.SaveAsync(contentType);

    UpdateContentTypePayload payload = new()
    {
      UniqueName = contentType.UniqueName.Value
    };
    UpdateContentTypeCommand command = new(_contentType.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ContentTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Null(exception.LanguageId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateContentTypePayload payload = new()
    {
      UniqueName = "123_BlogArticle"
    };
    UpdateContentTypeCommand command = new(_contentType.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(IdentifierValidator), error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing content type.")]
  public async Task It_should_update_an_existing_content_type()
  {
    _contentType.Description = new DescriptionUnit("This is the content type for blog article sub-titles.");
    _contentType.Update(ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    UpdateContentTypePayload payload = new()
    {
      DisplayName = new Modification<string>("  Blog Article  "),
      Description = new Modification<string>("    ")
    };
    UpdateContentTypeCommand command = new(_contentType.Id.ToGuid(), payload);
    ContentsType? contentType = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(contentType);

    Assert.Equal(_contentType.Id.ToGuid(), contentType.Id);
    Assert.Equal(_contentType.Version + 1, contentType.Version);
    Assert.Equal(Contracts.Actors.Actor.System, contentType.CreatedBy);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.True(contentType.CreatedOn < contentType.UpdatedOn);

    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.CleanTrim(), contentType.DisplayName);
    Assert.Null(contentType.Description);
  }
}

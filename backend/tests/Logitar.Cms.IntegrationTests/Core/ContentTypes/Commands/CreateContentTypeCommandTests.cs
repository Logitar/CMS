using FluentValidation.Results;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateContentTypeCommandTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  public CreateContentTypeCommandTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
  }

  [Fact(DisplayName = "It should create a new boolean content type.")]
  public async Task It_should_create_a_new_boolean_content_type()
  {
    CreateContentTypePayload payload = new("BlogArticle")
    {
      IsInvariant = false,
      DisplayName = "  Blog Article  ",
      Description = "    "
    };
    CreateContentTypeCommand command = new(payload);
    ContentsType contentType = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, contentType.Id);
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.True(contentType.CreatedOn < contentType.UpdatedOn);

    Assert.Equal(payload.IsInvariant, contentType.IsInvariant);
    Assert.Equal(payload.UniqueName.Trim(), contentType.UniqueName);
    Assert.Equal(payload.DisplayName?.CleanTrim(), contentType.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), contentType.Description);

    ContentTypeEntity? entity = await CmsContext.ContentTypes.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(contentType.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    ContentTypeAggregate contentType = new(new IdentifierUnit("BlogArticle"));
    await _contentTypeRepository.SaveAsync(contentType);

    CreateContentTypePayload payload = new(contentType.UniqueName.Value);
    CreateContentTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException<ContentTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal(nameof(payload.UniqueName), exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentTypePayload payload = new("123_SubTitle");
    CreateContentTypeCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(IdentifierValidator), error.ErrorCode);
    Assert.Equal("UniqueName", error.PropertyName);
  }
}

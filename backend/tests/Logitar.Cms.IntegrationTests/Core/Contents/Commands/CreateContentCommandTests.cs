using FluentValidation.Results;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Shared;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Contents.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateContentCommandTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  public CreateContentCommandTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
  }

  [Fact(DisplayName = "It should create a new invariant content.")]
  public async Task It_should_create_a_new_invariant_content()
  {
    ContentTypeAggregate contentType = new(new IdentifierUnit("BlogArticle"));
    await _contentTypeRepository.SaveAsync(contentType);

    CreateContentPayload payload = new(contentType.Id.ToGuid(), "prolongez-lete-avec-une-acura-nsx-coupe")
    {
      DisplayName = "  EXTEND YOUR SUMMER WITH AN ACURA NSX COUPE!  ",
      Description = "    "
    };
    CreateContentCommand command = new(payload);
    ContentItem contentItem = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(Guid.Empty, contentItem.Id);
    Assert.Equal(1, contentItem.Version);
    Assert.Equal(Actor.System, contentItem.CreatedBy);
    Assert.Equal(Actor.System, contentItem.UpdatedBy);
    Assert.Equal(contentItem.CreatedOn, contentItem.UpdatedOn);

    Assert.Equal(contentType.Id.ToGuid(), contentItem.ContentType.Id);

    ContentLocale invariant = Assert.Single(contentItem.Locales);
    Assert.Equal(payload.UniqueName.Trim(), invariant.UniqueName);
    //Assert.Equal(payload.DisplayName?.CleanTrim(), invariant.DisplayName); // TODO(fpion): DisplayName
    //Assert.Equal(payload.Description?.CleanTrim(), invariant.Description); // TODO(fpion): Description

    ContentItemEntity? contentItemEntity = await CmsContext.ContentItems.AsNoTracking().SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(contentItem.Id).Value);
    Assert.NotNull(contentItemEntity);
    Assert.NotNull(await CmsContext.ContentLocales.AsNoTracking().SingleOrDefaultAsync(x => x.ContentItemId == contentItemEntity.ContentItemId));
  }

  [Fact(DisplayName = "It should throw AggregateNotFoundException when the content type could not be found.")]
  public async Task It_should_throw_AggregateNotFoundException_when_the_content_type_could_not_be_found()
  {
    CreateContentPayload payload = new(contentTypeId: Guid.NewGuid(), uniqueName: Guid.NewGuid().ToString());
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<ContentTypeAggregate>>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.ContentTypeId.ToString(), exception.Id);
    Assert.Equal(nameof(payload.ContentTypeId), exception.PropertyName);
  }

  // TODO(fpion): ensure UniqueName unicity with ContentTypeId & LanguageId

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateContentPayload payload = new(contentTypeId: Guid.NewGuid(), uniqueName: $" {Guid.NewGuid()} ");
    CreateContentCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));
    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal(nameof(AllowedCharactersValidator), error.ErrorCode);
    Assert.Equal(nameof(payload.UniqueName), error.PropertyName);
  }
}

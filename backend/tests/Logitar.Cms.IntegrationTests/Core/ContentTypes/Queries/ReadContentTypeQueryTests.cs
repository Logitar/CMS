using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.ContentTypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadContentTypeQueryTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;

  private readonly ContentTypeAggregate _article;
  private readonly ContentTypeAggregate _category;

  public ReadContentTypeQueryTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();

    _article = new(new IdentifierUnit("BlogArticle"), isInvariant: false)
    {
      DisplayName = new DisplayNameUnit("Blog Article")
    };
    _article.Update();
    _category = new(new IdentifierUnit("BlogCategory"), isInvariant: true)
    {
      DisplayName = new DisplayNameUnit("Blog Category")
    };
    _category.Update();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _contentTypeRepository.SaveAsync([_article, _category]);
  }

  [Fact(DisplayName = "It should return null when the content type is not found.")]
  public async Task It_should_return_null_when_the_content_type_is_not_found()
  {
    ReadContentTypeQuery query = new(Id: Guid.NewGuid(), UniqueName: "Test");
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the content type found by ID.")]
  public async Task It_should_return_the_content_type_found_by_Id()
  {
    ReadContentTypeQuery query = new(_category.Id.ToGuid(), UniqueName: null);
    ContentsType? contentType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentType);
    Assert.Equal(_category.Id.ToGuid(), contentType.Id);
  }

  [Fact(DisplayName = "It should return the content type found by unique name.")]
  public async Task It_should_return_the_content_type_found_by_unique_name()
  {
    ReadContentTypeQuery query = new(Id: null, _article.UniqueName.Value);
    ContentsType? contentType = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(contentType);
    Assert.Equal(_article.Id.ToGuid(), contentType.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many content types are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_content_types_are_found()
  {
    ReadContentTypeQuery query = new(_category.Id.ToGuid(), _article.UniqueName.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<ContentsType>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

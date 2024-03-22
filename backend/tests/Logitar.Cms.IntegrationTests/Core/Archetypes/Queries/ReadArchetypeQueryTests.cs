using Logitar.Cms.Contracts.Archetypes;
using Logitar.Cms.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Archetypes.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadArchetypeQueryTests : IntegrationTests
{
  private readonly IArchetypeRepository _archetypeRepository;

  private readonly ArchetypeAggregate _articles;
  private readonly ArchetypeAggregate _products;

  public ReadArchetypeQueryTests() : base()
  {
    _archetypeRepository = ServiceProvider.GetRequiredService<IArchetypeRepository>();

    _articles = new(new IdentifierUnit("Article"));
    _products = new(new IdentifierUnit("Product"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _archetypeRepository.SaveAsync([_articles, _products]);
  }

  [Fact(DisplayName = "It should return null when the article is not found.")]
  public async Task It_should_return_null_when_the_article_is_not_found()
  {
    ReadArchetypeQuery query = new(Id: Guid.NewGuid(), UniqueName: "Test");
    Assert.Null(await Pipeline.ExecuteAsync(query));
  }

  [Fact(DisplayName = "It should return the article found by ID.")]
  public async Task It_should_return_the_article_found_by_Id()
  {
    ReadArchetypeQuery query = new(_articles.Id.ToGuid(), UniqueName: null);
    Archetype? archetype = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(archetype);
    Assert.Equal(_articles.Id.ToGuid(), archetype.Id);
  }

  [Fact(DisplayName = "It should return the article found by unique name.")]
  public async Task It_should_return_the_article_found_by_unique_name()
  {
    ReadArchetypeQuery query = new(Id: null, _products.UniqueName.Value);
    Archetype? archetype = await Pipeline.ExecuteAsync(query);
    Assert.NotNull(archetype);
    Assert.Equal(_products.Id.ToGuid(), archetype.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many articles are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_articles_are_found()
  {
    ReadArchetypeQuery query = new(_articles.Id.ToGuid(), _products.UniqueName.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Archetype>>(async () => await Pipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

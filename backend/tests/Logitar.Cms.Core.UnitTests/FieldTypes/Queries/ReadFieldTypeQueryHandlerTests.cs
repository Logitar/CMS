using Logitar.Cms.Contracts.FieldTypes;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadFieldTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();

  private readonly ReadFieldTypeQueryHandler _handler;

  public ReadFieldTypeQueryHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object);
  }

  [Theory(DisplayName = "It should return null when no field type is found.")]
  [InlineData(null, null)]
  [InlineData("f6488234-a9f2-45a0-b4d8-cdb6db17ad7d", "ArticleTitle")]
  public async Task It_should_return_null_when_no_field_type_is_found(string? id, string? uniqueName)
  {
    ReadFieldTypeQuery query = new(id == null ? null : Guid.Parse(id), uniqueName);
    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many field types are found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_field_types_are_found()
  {
    FieldType title = new("ArticleTitle")
    {
      Id = Guid.NewGuid()
    };
    _fieldTypeQuerier.Setup(x => x.ReadAsync(title.Id, _cancellationToken)).ReturnsAsync(title);

    FieldType body = new("ArticleBody")
    {
      Id = Guid.NewGuid()
    };
    _fieldTypeQuerier.Setup(x => x.ReadAsync(body.UniqueName, _cancellationToken)).ReturnsAsync(body);

    ReadFieldTypeQuery query = new(title.Id, body.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<FieldType>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

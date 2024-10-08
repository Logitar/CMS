using Logitar.Cms.Contracts.FieldTypes;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadFieldTypeQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();

  private readonly ReadFieldTypeQueryHandler _handler;

  private readonly FieldTypeModel _contents = new("Contents")
  {
    Id = Guid.NewGuid(),
    DataType = DataType.Text
  };
  private readonly FieldTypeModel _title = new("Title")
  {
    Id = Guid.NewGuid(),
    DataType = DataType.String
  };

  public ReadFieldTypeQueryHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object);

    _fieldTypeQuerier.Setup(x => x.ReadAsync(_contents.Id, _cancellationToken)).ReturnsAsync(_contents);
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_title.Id, _cancellationToken)).ReturnsAsync(_title);

    _fieldTypeQuerier.Setup(x => x.ReadAsync(_contents.UniqueName, _cancellationToken)).ReturnsAsync(_contents);
    _fieldTypeQuerier.Setup(x => x.ReadAsync(_title.UniqueName, _cancellationToken)).ReturnsAsync(_title);
  }

  [Fact(DisplayName = "It should return null when no fieldType could be found.")]
  public async Task It_should_return_null_when_no_fieldType_could_be_found()
  {
    ReadFieldTypeQuery query = new(Guid.NewGuid(), "Author");

    Assert.Null(await _handler.Handle(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the fieldType found by ID.")]
  public async Task It_should_return_the_fieldType_found_by_Id()
  {
    ReadFieldTypeQuery query = new(_title.Id, UniqueName: null);

    FieldTypeModel? fieldType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Same(_title, fieldType);
  }

  [Fact(DisplayName = "It should return the fieldType found by locale.")]
  public async Task It_should_return_the_fieldType_found_by_locale()
  {
    ReadFieldTypeQuery query = new(Id: null, _contents.UniqueName);

    FieldTypeModel? fieldType = await _handler.Handle(query, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Same(_contents, fieldType);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when too many field types were found.")]
  public async Task It_should_throw_TooManyResultsException_when_too_many_field_types_were_found()
  {
    ReadFieldTypeQuery query = new(_title.Id, _contents.UniqueName);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<FieldTypeModel>>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}

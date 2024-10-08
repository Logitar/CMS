using Logitar.Cms.Core.FieldTypes.Properties;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly SaveFieldTypeCommandHandler _handler;

  private readonly FieldType _fieldType = new("Contents", new TextProperties());

  public SaveFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeQuerier.Object, _fieldTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save the field type.")]
  public async Task It_should_save_the_field_type()
  {
    _fieldTypeQuerier.Setup(x => x.FindIdAsync(_fieldType.UniqueName, _cancellationToken)).ReturnsAsync(_fieldType.Id);

    SaveFieldTypeCommand command = new(_fieldType);

    await _handler.Handle(command, _cancellationToken);

    _fieldTypeQuerier.Verify(x => x.FindIdAsync(_fieldType.UniqueName, _cancellationToken), Times.Once);
    _fieldTypeRepository.Verify(x => x.SaveAsync(_fieldType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    FieldType conflict = new(_fieldType.UniqueName.Value, _fieldType.Properties);
    _fieldTypeQuerier.Setup(x => x.FindIdAsync(_fieldType.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    SaveFieldTypeCommand command = new(_fieldType);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _handler.Handle(command, _cancellationToken));
    Assert.Equal(typeof(FieldType).GetNamespaceQualifiedName(), exception.TypeName);
    Assert.Equal(_fieldType.Id.ToGuid(), exception.AggregateId);
    Assert.Equal(conflict.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(_fieldType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}

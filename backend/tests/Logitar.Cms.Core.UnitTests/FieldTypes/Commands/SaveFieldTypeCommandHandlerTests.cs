using Logitar.Cms.Core.FieldTypes.Properties;
using Logitar.Identity.Domain.Shared;
using Moq;

namespace Logitar.Cms.Core.FieldTypes.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveFieldTypeCommandHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly SaveFieldTypeCommandHandler _handler;

  public SaveFieldTypeCommandHandlerTests()
  {
    _handler = new(_fieldTypeRepository.Object);
  }

  [Fact(DisplayName = "It should save the field type.")]
  public async Task It_should_save_the_field_type()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.UniqueName, _cancellationToken)).ReturnsAsync(fieldType);

    SaveFieldTypeCommand command = new(fieldType);
    await _handler.Handle(command, _cancellationToken);

    _fieldTypeRepository.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw CmsUniqueNameAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_CmsUniqueNameAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    FieldTypeAggregate fieldType = new(new UniqueNameUnit(FieldTypeAggregate.UniqueNameSettings, "ArticleTitle"), new ReadOnlyStringProperties());
    FieldTypeAggregate other = new(fieldType.UniqueName, fieldType.Properties);
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.UniqueName, _cancellationToken)).ReturnsAsync(other);

    SaveFieldTypeCommand command = new(fieldType);
    var exception = await Assert.ThrowsAsync<CmsUniqueNameAlreadyUsedException<FieldTypeAggregate>>(
      async () => await _handler.Handle(command, _cancellationToken)
    );
    Assert.Equal(fieldType.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}

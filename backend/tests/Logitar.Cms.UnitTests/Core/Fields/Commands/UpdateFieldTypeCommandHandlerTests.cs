using FluentValidation;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Security.Cryptography;
using Moq;

namespace Logitar.Cms.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateFieldTypeCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IFieldTypeManager> _fieldTypeManager = new();
  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly UpdateFieldTypeCommandHandler _handler;

  public UpdateFieldTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _fieldTypeManager.Object, _fieldTypeQuerier.Object, _fieldTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Fact(DisplayName = "It should return null when updating a field type that does not exist.")]
  public async Task Given_NotExists_When_Handle_Then_NullReturned()
  {
    UpdateFieldTypePayload payload = new();
    UpdateFieldTypeCommand command = new(Guid.NewGuid(), payload);
    FieldTypeModel? fieldType = await _handler.Handle(command, _cancellationToken);
    Assert.Null(fieldType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    UpdateFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured?",
      DisplayName = new ChangeModel<string>(RandomStringGenerator.GetString(999)),
      Select = new SelectSettingsModel
      {
        Options = [new SelectOptionModel()]
      }
    };
    UpdateFieldTypeCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Select.Options[0].Text");
  }

  [Fact(DisplayName = "It should update an existing field type.")]
  public async Task Given_Exists_When_Handle_Then_FieldTypeUpdated()
  {
    UniqueName uniqueName = new(FieldType.UniqueNameSettings, "ArticleCategory");
    FieldType fieldType = new(uniqueName, new SelectSettings(isMultiple: true, options: [new SelectOption("Software Architecture")]));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new ChangeModel<string>(" Article Category "),
      Description = new ChangeModel<string>("  This is the field type for blog article categories.  "),
      Select = new SelectSettingsModel
      {
        IsMultiple = false,
        Options =
        [
          new SelectOptionModel
          {
            Text = "Software Architecture",
            Value = "software-architecture"
          },
          new SelectOptionModel
          {
            Text = "Linux Administration",
            Value = "linux-admin"
          }
        ]
      }
    };
    UpdateFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload);
    FieldTypeModel? result = await _handler.Handle(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(model, result);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), fieldType.Description?.Value);

    SelectSettings? settings = fieldType.Settings as SelectSettings;
    Assert.NotNull(settings);
    Assert.Equal(payload.Select.IsMultiple, settings.IsMultiple);
    foreach (SelectOptionModel option in payload.Select.Options)
    {
      Assert.Contains(settings.Options, o => o.Text == option.Text && o.Value == option.Value);
    }
  }
}

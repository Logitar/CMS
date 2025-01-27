﻿using FluentValidation;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Security.Cryptography;
using Moq;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Cms.Core.Fields.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceFieldTypeCommandHandlerTests
{
  private readonly ActorId _actorId = ActorId.NewId();
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IFieldTypeManager> _fieldTypeManager = new();
  private readonly Mock<IFieldTypeQuerier> _fieldTypeQuerier = new();
  private readonly Mock<IFieldTypeRepository> _fieldTypeRepository = new();

  private readonly CreateOrReplaceFieldTypeCommandHandler _handler;

  public CreateOrReplaceFieldTypeCommandHandlerTests()
  {
    _handler = new(_applicationContext.Object, _fieldTypeManager.Object, _fieldTypeQuerier.Object, _fieldTypeRepository.Object);

    _applicationContext.Setup(x => x.ActorId).Returns(_actorId);
  }

  [Theory(DisplayName = "It should create a new Boolean field type.")]
  [InlineData(null)]
  [InlineData("83128db6-4343-40a1-8c4b-de2668eb1700")]
  public async Task Given_BooleanNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      DisplayName = " Is featured? ",
      Description = "  This is the field type for blog article feature marker.  ",
      Boolean = new BooleanSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Boolean && y.Settings is BooleanSettings),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new DateTime field type.")]
  [InlineData(null)]
  [InlineData("eb62f571-505c-497f-8723-f74a017e4ca5")]
  public async Task Given_DateTimeNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "PublicationDate",
      DisplayName = " Publication Date ",
      Description = "  This is the field type for blog article original publication dates.  ",
      DateTime = new DateTimeSettingsModel
      {
        MinimumValue = new DateTime(2000, 1, 1),
        MaximumValue = new DateTime(2024, 12, 31, 23, 59, 59)
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.DateTime
        && ((DateTimeSettings)y.Settings).MinimumValue == payload.DateTime.MinimumValue
        && ((DateTimeSettings)y.Settings).MaximumValue == payload.DateTime.MaximumValue),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Number field type.")]
  [InlineData(null)]
  [InlineData("9989da9f-3d34-42a6-b283-e65195d7905d")]
  public async Task Given_NumberNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "WordCount",
      DisplayName = " Word Count ",
      Description = "  This is the field type for blog article word counts.  ",
      Number = new NumberSettingsModel
      {
        MinimumValue = 1.0,
        Step = 1.0
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Number
        && ((NumberSettings)y.Settings).MinimumValue == payload.Number.MinimumValue
        && ((NumberSettings)y.Settings).MaximumValue == payload.Number.MaximumValue
        && ((NumberSettings)y.Settings).Step == payload.Number.Step),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new RelatedContent field type.")]
  [InlineData(null)]
  [InlineData("f8245cf4-b6de-4771-9349-ff07a3e681b5")]
  public async Task Given_RelatedContentNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleAuthor",
      DisplayName = " Article Author ",
      Description = "  This is the field type for blog article authors.  ",
      RelatedContent = new RelatedContentSettingsModel
      {
        ContentTypeId = Guid.NewGuid(),
        IsMultiple = true
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.RelatedContent
        && ((RelatedContentSettings)y.Settings).ContentTypeId.ToGuid() == payload.RelatedContent.ContentTypeId
        && ((RelatedContentSettings)y.Settings).IsMultiple == payload.RelatedContent.IsMultiple),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new RichText field type.")]
  [InlineData(null)]
  [InlineData("413e58aa-b2cb-4bd9-a5d4-7f0a89e2d9b6")]
  public async Task Given_RichTextNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleContent",
      DisplayName = " Article Content ",
      Description = "  This is the field type for blog article contents.  ",
      RichText = new RichTextSettingsModel
      {
        ContentType = MediaTypeNames.Text.Plain,
        MinimumLength = 1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.RichText
        && ((RichTextSettings)y.Settings).ContentType == payload.RichText.ContentType
        && ((RichTextSettings)y.Settings).MinimumLength == payload.RichText.MinimumLength
        && ((RichTextSettings)y.Settings).MaximumLength == payload.RichText.MaximumLength),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Select field type.")]
  [InlineData(null)]
  [InlineData("60746da6-36f6-4cdf-8992-4a7d244d192e")]
  public async Task Given_SelectNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleCategory",
      DisplayName = " Article Categtory ",
      Description = "  This is the field type for blog article categories.  ",
      Select = new SelectSettingsModel
      {
        IsMultiple = true,
        Options =
        [
          new SelectOptionModel
          {
            Text = "Software Architecture",
            Value = "software-architecture"
          }
        ]
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Select
        && ((SelectSettings)y.Settings).IsMultiple == payload.Select.IsMultiple
        && ((SelectSettings)y.Settings).Options.Single().Equals(new SelectOption(payload.Select.Options.Single()))),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new String field type.")]
  [InlineData(null)]
  [InlineData("556d1ca3-3692-4600-89ff-1c22ced24d69")]
  public async Task Given_StringNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field type for blog article titles.  ",
      String = new StringSettingsModel
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.String
        && ((StringSettings)y.Settings).MinimumLength == payload.String.MinimumLength
        && ((StringSettings)y.Settings).MaximumLength == payload.String.MaximumLength
        && ((StringSettings)y.Settings).Pattern == payload.String.Pattern),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should create a new Tags field type.")]
  [InlineData(null)]
  [InlineData("1731da09-02e3-4ce8-850c-106817127410")]
  public async Task Given_TagsNotExists_When_Handle_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(It.IsAny<FieldType>(), _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleKeywords",
      DisplayName = " Article Keywords ",
      Description = "  This is the field type for blog article keywords.  ",
      Tags = new TagsSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(
      It.Is<FieldType>(y => (!id.HasValue || y.Id.ToGuid() == id.Value) && y.UniqueName.Value == payload.UniqueName
        && y.CreatedBy == _actorId && y.UpdatedBy == _actorId
        && y.DisplayName != null && y.DisplayName.Value == payload.DisplayName.Trim()
        && y.Description != null && y.Description.Value == payload.Description.Trim()
        && y.DataType == DataType.Tags && y.Settings is TagsSettings),
      _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "It should replace/update an existing Boolean field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_BooleanExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "IsFeatured"), new BooleanSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Featured");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      DisplayName = " Is featured? ",
      Description = "  This is the field type for blog article feature marker.  ",
      Boolean = new BooleanSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Boolean, new BooleanSettingsModel((BooleanSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing DateTime field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_DateTimeExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "PublicationDate"), new DateTimeSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "PublishedOn");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "PublicationDate",
      DisplayName = " Publication Date ",
      Description = "  This is the field type for blog article original publication dates.  ",
      DateTime = new DateTimeSettingsModel
      {
        MinimumValue = new DateTime(2000, 1, 1),
        MaximumValue = new DateTime(2024, 12, 31, 23, 59, 59)
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.DateTime, new DateTimeSettingsModel((DateTimeSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing Number field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NumberExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "WordCount"), new NumberSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Words");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "WordCount",
      DisplayName = " Word Count ",
      Description = "  This is the field type for blog article word counts.  ",
      Number = new NumberSettingsModel
      {
        MinimumValue = 1.0,
        Step = 1.0
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Number, new NumberSettingsModel((NumberSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing RelatedContent field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RelatedContentExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleAuthor"), new RelatedContentSettings(new ContentTypeId(), isMultiple: false));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Author");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleAuthor",
      DisplayName = " Article Author ",
      Description = "  This is the field type for blog article authors.  ",
      RelatedContent = new RelatedContentSettingsModel
      {
        ContentTypeId = Guid.NewGuid(),
        IsMultiple = true
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.RelatedContent, new RelatedContentSettingsModel((RelatedContentSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing RichText field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RichTextExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleContent"), new RichTextSettings(MediaTypeNames.Text.Plain));
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Contents");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleContent",
      DisplayName = " Article Content ",
      Description = "  This is the field type for blog article contents.  ",
      RichText = new RichTextSettingsModel
      {
        ContentType = MediaTypeNames.Text.Plain,
        MinimumLength = 1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.RichText, new RichTextSettingsModel((RichTextSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing Select field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_SelectExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleCategory"), new SelectSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Category");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleCategory",
      DisplayName = " Article Categtory ",
      Description = "  This is the field type for blog article categories.  ",
      Select = new SelectSettingsModel
      {
        IsMultiple = true,
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
            Value = "linux-administration"
          }
        ]
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);

    SelectSettings? settings = fieldType.Settings as SelectSettings;
    Assert.NotNull(settings);
    Assert.Equal(payload.Select.IsMultiple, settings.IsMultiple);
    foreach (SelectOptionModel option in payload.Select.Options)
    {
      Assert.Contains(settings.Options, o => o.Text == option.Text && o.Value == option.Value);
    }
  }

  [Theory(DisplayName = "It should replace/update an existing String field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_StringExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Title");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleTitle",
      DisplayName = " Article Title ",
      Description = "  This is the field type for blog article titles.  ",
      String = new StringSettingsModel
      {
        MinimumLength = 1,
        MaximumLength = 100
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.String, new StringSettingsModel((StringSettings)fieldType.Settings));
  }

  [Theory(DisplayName = "It should replace/update an existing Tags field type.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_TagsExists_When_Handle_Then_ReplacedOrUpdated(bool update)
  {
    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleKeywords"), new TagsSettings());
    _fieldTypeRepository.Setup(x => x.LoadAsync(fieldType.Id, _cancellationToken)).ReturnsAsync(fieldType);

    UniqueName? uniqueName = null;
    long? version = null;
    if (update)
    {
      version = fieldType.Version;

      FieldType reference = new(fieldType.UniqueName, fieldType.Settings, fieldType.CreatedBy, fieldType.Id);
      _fieldTypeRepository.Setup(x => x.LoadAsync(reference.Id, version, _cancellationToken)).ReturnsAsync(reference);

      uniqueName = new(FieldType.UniqueNameSettings, "Keywords");
      fieldType.SetUniqueName(uniqueName, _actorId);
    }

    FieldTypeModel model = new();
    _fieldTypeQuerier.Setup(x => x.ReadAsync(fieldType, _cancellationToken)).ReturnsAsync(model);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleKeywords",
      DisplayName = " Article Keywords ",
      Description = "  This is the field type for blog article keywords.  ",
      Tags = new TagsSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(fieldType.Id.ToGuid(), payload, version);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.FieldType);
    Assert.Same(model, result.FieldType);

    _fieldTypeManager.Verify(x => x.SaveAsync(fieldType, _cancellationToken), Times.Once);

    Assert.Equal(_actorId, fieldType.UpdatedBy);
    Assert.Equal(uniqueName?.Value ?? payload.UniqueName, fieldType.UniqueName.Value);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), fieldType.Description?.Value);
    Assert.Equal(payload.Tags, new TagsSettingsModel((TagsSettings)fieldType.Settings));
  }

  [Fact(DisplayName = "It should return null when updating a field type that does not exist.")]
  public async Task Given_NotExistsWithVersion_When_Handle_Then_EmptyResult()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      Boolean = new BooleanSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(Guid.NewGuid(), payload, Version: -1);
    CreateOrReplaceFieldTypeResult result = await _handler.Handle(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.FieldType);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_Handle_Then_ValidationException()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured?",
      DisplayName = RandomStringGenerator.GetString(999),
      RichText = new RichTextSettingsModel
      {
        ContentType = "text"
      },
      String = new StringSettingsModel
      {
        MinimumLength = -1
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "AllowedCharactersValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "ContentTypeValidator" && e.PropertyName == "RichText.ContentType");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanValidator" && e.PropertyName == "String.MinimumLength.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "CreateOrReplaceFieldTypeValidator");
  }
}

﻿using FluentValidation;
using FluentValidation.Results;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Contents;

[Trait(Traits.Category, Categories.Unit)]
public class ContentTypeTests
{
  [Fact(DisplayName = "SetField: it should not do anything when the field definition had no change.")]
  public void Given_FieldDefinitionNoChange_When_SetField_Then_NoChange()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringSettings(minimumLength: 1, maximumLength: 100));
    FieldDefinition field1 = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(field1);
    contentType.ClearChanges();

    FieldDefinition field2 = new(field1.Id, field1.FieldTypeId, field1.IsInvariant, field1.IsRequired, field1.IsIndexed, field1.IsUnique,
      field1.UniqueName, field1.DisplayName, field1.Description, field1.Placeholder);
    contentType.SetField(field2);
    Assert.False(contentType.HasChanges);
    Assert.Empty(contentType.Changes);
  }

  [Fact(DisplayName = "SetField: it should throw UniqueNameAlreadyUsedException when the field definition unique name is already used.")]
  public void Given_FieldDefinitionNameConflict_When_SetField_Then_UniqueNameAlreadyUsedException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false);

    FieldType fieldType = new(new UniqueName(FieldType.UniqueNameSettings, "ArticleTitle"), new StringSettings(minimumLength: 1, maximumLength: 100));
    FieldDefinition conflict = new(Guid.NewGuid(), fieldType.Id, IsInvariant: false, IsRequired: true, IsIndexed: true, IsUnique: false,
      new Identifier("ArticleTitle"), DisplayName: null, Description: null, Placeholder: null);
    contentType.SetField(conflict);

    FieldDefinition field = new(Guid.NewGuid(), conflict.FieldTypeId, conflict.IsInvariant, conflict.IsRequired, conflict.IsIndexed, conflict.IsUnique,
      conflict.UniqueName, conflict.DisplayName, conflict.Description, conflict.Placeholder);
    var exception = Assert.Throws<UniqueNameAlreadyUsedException>(() => contentType.SetField(field));
    Assert.Equal(conflict.Id, exception.ConflictId);
    Assert.Equal(field.Id, exception.EntityId);
    Assert.Equal(field.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "SetField: it should throw ValidationException when setting a variant field definition on an invariant content type.")]
  public void Given_InvariantContentTypeVariantFieldDefinition_When_SetField_Then_ValidationException()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"));

    FieldDefinition field = new(
      Guid.NewGuid(),
      FieldTypeId.NewId(),
      IsInvariant: false,
      IsRequired: false,
      IsIndexed: false,
      IsUnique: false,
      new Identifier("Biography"),
      DisplayName: null,
      Description: null,
      Placeholder: null);
    var exception = Assert.Throws<ValidationException>(() => contentType.SetField(field));

    ValidationFailure failure = Assert.Single(exception.Errors);
    Assert.Equal(field.IsInvariant, failure.AttemptedValue);
    Assert.Equal("InvariantValidator", failure.ErrorCode);
    Assert.Equal("'IsInvariant' must be true. Invariant content types cannot define variant fields.", failure.ErrorMessage);
    Assert.Equal("IsInvariant", failure.PropertyName);
  }
}

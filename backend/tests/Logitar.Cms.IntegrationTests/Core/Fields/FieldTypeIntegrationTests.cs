using Logitar.Cms.Core.Fields.Commands;
using Logitar.Cms.Core.Fields.Models;
using System.Net.Mime; // NOTE(fpion): cannot be added to CSPROJ due to ContentType aggregate.

namespace Logitar.Cms.Core.Fields;

[Trait(Traits.Category, Categories.Integration)]
public class FieldTypeIntegrationTests : IntegrationTests
{
  public FieldTypeIntegrationTests() : base()
  {
  }

  [Theory(DisplayName = "It should create a new Boolean field type.")]
  [InlineData(null)]
  [InlineData("5796aab5-bf51-469e-93ff-1a1718a55d1a")]
  public async Task Given_BooleanType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "IsFeatured",
      DisplayName = " Is featured? ",
      Description = "  This is the field type for blog article featuring marker.  ",
      Boolean = new BooleanSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.Boolean, fieldType.DataType);
    Assert.Equal(payload.Boolean, fieldType.Boolean);
  }

  [Theory(DisplayName = "It should create a new DateTime field type.")]
  [InlineData(null)]
  [InlineData("daf4bf4b-c451-4afb-baaf-fb69ef3ef585")]
  public async Task Given_DateTimeType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "PublicationDate",
      DisplayName = " Published On ",
      Description = "  This is the field type for blog article publication dates.  ",
      DateTime = new DateTimeSettingsModel
      {
        MinimumValue = new DateTime(2000, 1, 1),
        MaximumValue = new DateTime(2024, 12, 31, 23, 59, 0)
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.DateTime, fieldType.DataType);
    Assert.Equal(payload.DateTime, fieldType.DateTime);
  }

  [Theory(DisplayName = "It should create a new Number field type.")]
  [InlineData(null)]
  [InlineData("0f3d9db9-93f3-4593-b538-68962460e473")]
  public async Task Given_NumberType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "WordCount",
      DisplayName = " Word Count ",
      Description = "  This is the field type for blog article word count.  ",
      Number = new NumberSettingsModel
      {
        MinimumValue = 1.0,
        Step = 1.0
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.Number, fieldType.DataType);
    Assert.Equal(payload.Number, fieldType.Number);
  }

  [Theory(DisplayName = "It should create a new RichText field type.")]
  [InlineData(null)]
  [InlineData("2a7357dd-4996-4f85-9159-20da1acdca76")]
  public async Task Given_RichTextType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "ArticleContents",
      DisplayName = " Article Contents ",
      Description = "  This is the field type for blog article contents.  ",
      RichText = new RichTextSettingsModel
      {
        ContentType = MediaTypeNames.Text.Plain
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.RichText, fieldType.DataType);
    Assert.Equal(payload.RichText, fieldType.RichText);
  }

  [Theory(DisplayName = "It should create a new Select field type.")]
  [InlineData(null)]
  [InlineData("71bbbde7-c34c-4428-afd0-8ebd820b6955")]
  public async Task Given_SelectType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "OnSale",
      DisplayName = " On-sale? ",
      Description = "  A field type to mark items on-sale.  ",
      Select = new SelectSettingsModel
      {
        IsMultiple = true,
        Options =
        [
          new SelectOptionModel { IsDisabled = true, Text = "None" },
          new SelectOptionModel { IsSelected = true, Text = "May be?", Value = "may_be" },
          new SelectOptionModel { Text = "True", Value = "true" },
          new SelectOptionModel { Text = "False", Value = "false" }
        ]
      }
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.Select, fieldType.DataType);

    Assert.NotNull(fieldType.Select);
    Assert.Equal(payload.Select.IsMultiple, fieldType.Select.IsMultiple);
    Assert.True(payload.Select.Options.SequenceEqual(fieldType.Select.Options));
  }

  [Theory(DisplayName = "It should create a new String field type.")]
  [InlineData(null)]
  [InlineData("06854434-c633-403b-b09f-cebca4513d03")]
  public async Task Given_StringType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

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
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.String, fieldType.DataType);
    Assert.Equal(payload.String, fieldType.String);
  }

  [Theory(DisplayName = "It should create a new Tags field type.")]
  [InlineData(null)]
  [InlineData("a7329767-ee3e-4546-9606-85466c1d1b82")]
  public async Task Given_TagsType_When_Create_Then_FieldTypeCreated(string? idValue)
  {
    Guid? id = idValue == null ? null : Guid.Parse(idValue);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "MetaKeywords",
      DisplayName = " Keywords (meta) ",
      Description = "  This is the field type keyword metadata.  ",
      Tags = new TagsSettingsModel()
    };
    CreateOrReplaceFieldTypeCommand command = new(id, payload, Version: null);
    CreateOrReplaceFieldTypeResult result = await Mediator.Send(command);
    Assert.True(result.Created);

    FieldTypeModel? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    if (id.HasValue)
    {
      Assert.Equal(id.Value, fieldType.Id);
    }
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn, TimeSpan.FromMinutes(1));

    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.Tags, fieldType.DataType);
    Assert.Equal(payload.Tags, fieldType.Tags);
  }
}

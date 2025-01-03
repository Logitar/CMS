using Logitar.Cms.Core.Actors;
using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Models;
using Logitar.Cms.Core.Fields.Commands;
using Logitar.Cms.Core.Fields.Models;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.Identity.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core.Fields;

[Trait(Traits.Category, Categories.Integration)]
public class FieldDefinitionIntegrationTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  private readonly FieldType _sku;
  private readonly ContentType _cymbal;

  public FieldDefinitionIntegrationTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();

    _sku = new(new UniqueName(FieldType.UniqueNameSettings, "Sku"), new StringSettings(minimumLength: 1, maximumLength: 10));
    _cymbal = new(new Identifier("Cymbal"), isInvariant: false);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _fieldTypeRepository.SaveAsync(_sku);
    await _contentTypeRepository.SaveAsync(_cymbal);
  }

  [Theory(DisplayName = "It should create a new field definition.")]
  [InlineData(null)]
  [InlineData("440a4a81-7a6e-453d-8c2b-b1d2d1e165bf")]
  public async Task Given_NotExist_When_Create_Then_FieldDefinitionCreated(string? fieldIdValue)
  {
    Guid? fieldId = fieldIdValue == null ? null : Guid.Parse(fieldIdValue);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldTypeId = _sku.Id.ToGuid(),
      IsInvariant = false,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = true,
      UniqueName = "Sku",
      DisplayName = " SKU ",
      Description = "  In inventory management, a stock keeping unit (abbreviated as SKU) is the unit of measure in which the stocks of a material are managed. It is a distinct type of item for sale, purchase, or tracking in inventory, such as a product or service, and all attributes associated with the item type that distinguish it from other item types (for a product, these attributes can include manufacturer, description, material, size, color, packaging, and warranty terms). When a business records the inventory of its stock, it counts the quantity it has of each unit, or SKU.  ",
      Placeholder = " Enter the SKU of the cymbal in this input. "
    };
    CreateOrReplaceFieldDefinitionCommand command = new(_cymbal.Id.ToGuid(), fieldId, payload);
    ContentTypeModel? contentType = await Mediator.Send(command);

    Assert.NotNull(contentType);
    Assert.Equal(command.ContentTypeId, contentType.Id);
    Assert.Equal(_cymbal.Version + 1, contentType.Version);
    Assert.Equal(new ActorModel(), contentType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.CreatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromMinutes(1));
    Assert.Equal(1, contentType.FieldCount);

    FieldDefinitionModel field = Assert.Single(contentType.Fields);
    if (command.FieldId.HasValue)
    {
      Assert.Equal(command.FieldId.Value, field.Id);
      Assert.Equal(0, field.Order);
      Assert.Equal(payload.FieldTypeId, field.FieldType?.Id);
      Assert.Equal(payload.IsInvariant, field.IsInvariant);
      Assert.Equal(payload.IsRequired, field.IsRequired);
      Assert.Equal(payload.IsIndexed, field.IsIndexed);
      Assert.Equal(payload.IsUnique, field.IsUnique);
      Assert.Equal(payload.UniqueName, field.UniqueName);
      Assert.Equal(payload.DisplayName.Trim(), field.DisplayName);
      Assert.Equal(payload.Description.Trim(), field.Description);
      Assert.Equal(payload.Placeholder.Trim(), field.Placeholder);
    }
  }
}

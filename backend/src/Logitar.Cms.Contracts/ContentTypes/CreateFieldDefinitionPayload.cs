namespace Logitar.Cms.Contracts.ContentTypes;

public record CreateFieldDefinitionPayload
{
  public Guid ContentTypeId { get; set; }

  public Guid FieldTypeId { get; set; }

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public CreateFieldDefinitionPayload() : this(Guid.Empty, Guid.Empty, string.Empty)
  {
  }

  public CreateFieldDefinitionPayload(Guid contentTypeId, Guid fieldTypeId, string uniqueName)
  {
    ContentTypeId = contentTypeId;
    FieldTypeId = fieldTypeId;
    UniqueName = uniqueName;
  }
}

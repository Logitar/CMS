using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Fields;

namespace Logitar.Cms.Contracts.ContentTypes;

public class FieldDefinition
{
  public Guid Id { get; set; }

  public ContentsType ContentType { get; set; }
  public int Order { get; set; }

  public FieldType FieldType { get; set; }

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public Actor CreatedBy { get; set; } = Actor.System;
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = Actor.System;
  public DateTime UpdatedOn { get; set; }

  public FieldDefinition() : this(new ContentsType(), new FieldType(), string.Empty)
  {
  }

  public FieldDefinition(ContentsType contentType, FieldType fieldType, string uniqueName)
  {
    ContentType = contentType;
    FieldType = fieldType;
    UniqueName = uniqueName;
  }
}

namespace Logitar.Cms.Core.Contents.Models;

public record CreateContentPayload
{
  public Guid? Id { get; set; }

  public Guid ContentTypeId { get; set; }

  public Guid? LanguageId { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}

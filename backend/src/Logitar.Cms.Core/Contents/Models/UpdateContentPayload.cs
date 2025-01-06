namespace Logitar.Cms.Core.Contents.Models;

public record UpdateContentPayload
{
  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  // TODO(fpion): FieldValues
}

namespace Logitar.Cms.Contracts.Contents;

public record FieldValuePayload
{
  public Guid Id { get; set; }
  public string Value { get; set; }

  public FieldValuePayload() : this(Guid.Empty, string.Empty)
  {
  }

  public FieldValuePayload(Guid id, string value)
  {
    Id = id;
    Value = value;
  }
}

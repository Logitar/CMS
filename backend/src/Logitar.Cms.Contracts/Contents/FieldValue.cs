namespace Logitar.Cms.Contracts.Contents;

public record FieldValue
{
  public Guid Id { get; set; }
  public string Value { get; set; }

  public FieldValue() : this(Guid.Empty, string.Empty)
  {
  }

  public FieldValue(KeyValuePair<Guid, string> field) : this(field.Key, field.Value)
  {
  }

  public FieldValue(Guid id, string value)
  {
    Id = id;
    Value = value;
  }
}

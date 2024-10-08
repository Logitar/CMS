using Logitar.Cms.Contracts.FieldTypes.Properties;

namespace Logitar.Cms.Contracts.FieldTypes;

public class FieldTypeModel : AggregateModel
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public DataType DataType { get; set; }
  public StringPropertiesModel? StringProperties { get; set; }
  public TextPropertiesModel? TextProperties { get; set; }

  public FieldTypeModel() : this(string.Empty)
  {
  }

  public FieldTypeModel(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} (Id={Id})";
}

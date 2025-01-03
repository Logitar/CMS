namespace Logitar.Cms.Core.Fields;

public class FieldTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified field type could not be found.";

  public Guid FieldTypeId
  {
    get => (Guid)Data[nameof(FieldTypeId)]!;
    private set => Data[nameof(FieldTypeId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data.Add(nameof(FieldTypeId), FieldTypeId);
      error.Data.Add(nameof(PropertyName), PropertyName);
      return error;
    }
  }

  public FieldTypeNotFoundException(FieldTypeId fieldTypeId, string propertyName) : base(BuildMessage(fieldTypeId, propertyName))
  {
    FieldTypeId = fieldTypeId.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(FieldTypeId fieldTypeId, string propertyName) => new ErrorMessageBuilder()
    .AddData(nameof(FieldTypeId), fieldTypeId.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

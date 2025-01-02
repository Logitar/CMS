using Logitar.Cms.Core.Fields.Settings;

namespace Logitar.Cms.Core.Fields;

public class UnexpectedFieldTypeSettingsException : BadRequestException
{
  private const string ErrorMessage = "The specified field type settings were not expected.";

  public Guid FieldTypeId
  {
    get => (Guid)Data[nameof(FieldTypeId)]!;
    private set => Data[nameof(FieldTypeId)] = value;
  }
  public DataType ExpectedDataType
  {
    get => (DataType)Data[nameof(ExpectedDataType)]!;
    private set => Data[nameof(ExpectedDataType)] = value;
  }
  public DataType ActualDataType
  {
    get => (DataType)Data[nameof(ActualDataType)]!;
    private set => Data[nameof(ActualDataType)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(ExpectedDataType)] = ExpectedDataType;
      error.Data[nameof(ActualDataType)] = ActualDataType;
      return error;
    }
  }

  public UnexpectedFieldTypeSettingsException(FieldType fieldType, FieldTypeSettings settings) : base(BuildMessage(fieldType, settings))
  {
    FieldTypeId = fieldType.Id.ToGuid();
    ExpectedDataType = fieldType.DataType;
    ActualDataType = settings.DataType;
  }

  private static string BuildMessage(FieldType fieldType, FieldTypeSettings settings) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(FieldTypeId), fieldType.Id.ToGuid())
    .AddData(nameof(ExpectedDataType), fieldType.DataType)
    .AddData(nameof(ActualDataType), settings.DataType)
    .Build();
}

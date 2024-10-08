using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes;

public class DataTypeMismatchException : Exception
{
  public const string ErrorMessage = "The specified data type was not expected.";

  public Guid FieldTypeId
  {
    get => (Guid)Data[nameof(FieldTypeId)]!;
    private set => Data[nameof(FieldTypeId)] = value;
  }
  public DataType ExpectedType
  {
    get => (DataType)Data[nameof(ExpectedType)]!;
    private set => Data[nameof(ExpectedType)] = value;
  }
  public DataType ActualType
  {
    get => (DataType)Data[nameof(ActualType)]!;
    private set => Data[nameof(ActualType)] = value;
  }

  public DataTypeMismatchException(FieldType fieldType, DataType actualType)
    : base(BuildMessage(fieldType, actualType))
  {
    FieldTypeId = fieldType.Id.ToGuid();
    ExpectedType = fieldType.DataType;
    ActualType = actualType;
  }

  private static string BuildMessage(FieldType fieldType, DataType actualType) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(FieldTypeId), fieldType.Id)
    .AddData(nameof(ExpectedType), fieldType.DataType)
    .AddData(nameof(ActualType), actualType)
    .Build();
}

using Logitar.Cms.Contracts.FieldTypes;

namespace Logitar.Cms.Core.FieldTypes;

public class DataTypeMismatchException : Exception
{
  public const string ErrorMessage = "The specified data type was not expected.";

  public string FieldTypeId
  {
    get => (string)Data[nameof(FieldTypeId)]!;
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

  public DataTypeMismatchException(FieldTypeAggregate fieldType, DataType actualType)
    : base(BuildMessage(fieldType, actualType))
  {
    FieldTypeId = fieldType.Id.Value;
    ExpectedType = fieldType.DataType;
    ActualType = actualType;
  }

  private static string BuildMessage(FieldTypeAggregate fieldType, DataType actualType) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(FieldTypeId), fieldType.Id.Value)
    .AddData(nameof(ExpectedType), fieldType.DataType)
    .AddData(nameof(ActualType), actualType)
    .Build();
}

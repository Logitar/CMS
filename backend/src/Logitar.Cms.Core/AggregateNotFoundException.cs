namespace Logitar.Cms.Core;

public class AggregateNotFoundException : Exception
{
  public const string ErrorMessage = "The specified aggregate could not be found.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public AggregateNotFoundException(Type type, string id, string? propertyName = null) : base(BuildMessage(type, id, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, string id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}

public class AggregateNotFoundException<T> : AggregateNotFoundException
{
  public AggregateNotFoundException(Guid id, string? propertyName = null) : this(id.ToString(), propertyName)
  {
  }

  public AggregateNotFoundException(string id, string? propertyName = null) : base(typeof(T), id, propertyName)
  {
  }
}

namespace Logitar.Cms.Core.Shared;

public class IdentifierAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified identifier is already used.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string Identifier
  {
    get => (string)Data[nameof(Identifier)]!;
    private set => Data[nameof(Identifier)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public IdentifierAlreadyUsedException(Type type, IdentifierUnit identifier, string? propertyName = null) : base(BuildMessage(type, identifier, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    Identifier = identifier.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, IdentifierUnit identifier, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
  .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
  .AddData(nameof(Identifier), identifier.Value)
  .AddData(nameof(PropertyName), propertyName, "<null>")
  .Build();
}

public class IdentifierAlreadyUsedException<T> : IdentifierAlreadyUsedException
{
  public IdentifierAlreadyUsedException(IdentifierUnit identifier, string? propertyName = null) : base(typeof(T), identifier, propertyName)
  {
  }
}

namespace Logitar.Cms.Core.Shared;

public class UniqueNameAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified identifier is already used.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UniqueNameAlreadyUsedException(Type type, IdentifierUnit uniqueName, string? propertyName = null) : base(BuildMessage(type, uniqueName, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, IdentifierUnit uniqueName, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
  .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
  .AddData(nameof(UniqueName), uniqueName.Value)
  .AddData(nameof(PropertyName), propertyName, "<null>")
  .Build();
}

public class UniqueNameAlreadyUsedException<T> : UniqueNameAlreadyUsedException
{
  public UniqueNameAlreadyUsedException(IdentifierUnit uniqueName, string? propertyName = null) : base(typeof(T), uniqueName, propertyName)
  {
  }
}

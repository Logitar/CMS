using Logitar.Cms.Contracts.Errors;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core;

public class CmsUniqueNameAlreadyUsedException : ConflictException
{
  public const string ErrorMessage = "The specified unique name is already used.";

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

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PropertyName, UniqueName);

  public CmsUniqueNameAlreadyUsedException(Type type, IdentifierUnit uniqueName, string? propertyName = null)
    : base(BuildMessage(type, uniqueName.Value, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  public CmsUniqueNameAlreadyUsedException(Type type, UniqueNameUnit uniqueName, string? propertyName = null)
    : base(BuildMessage(type, uniqueName.Value, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, string uniqueName, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}

public class CmsUniqueNameAlreadyUsedException<T> : CmsUniqueNameAlreadyUsedException
{
  public CmsUniqueNameAlreadyUsedException(IdentifierUnit uniqueName, string? propertyName = null)
    : base(typeof(T), uniqueName, propertyName)
  {
  }
  public CmsUniqueNameAlreadyUsedException(UniqueNameUnit uniqueName, string? propertyName = null)
    : base(typeof(T), uniqueName, propertyName)
  {
  }
}

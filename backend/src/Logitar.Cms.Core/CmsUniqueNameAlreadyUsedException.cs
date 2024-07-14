using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core.Languages;
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
  public string? LanguageId
  {
    get => (string?)Data[nameof(LanguageId)];
    private set => Data[nameof(LanguageId)] = value;
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

  public override Error Error
  {
    get
    {
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, PropertyName, UniqueName);
      if (LanguageId != null)
      {
        error.Add(new ErrorData(nameof(LanguageId), new LanguageId(LanguageId).ToGuid().ToString()));
      }
      return error;
    }
  }

  public CmsUniqueNameAlreadyUsedException(Type type, IdentifierUnit uniqueName, string? propertyName = null)
    : base(BuildMessage(type, languageId: null, uniqueName.Value, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  public CmsUniqueNameAlreadyUsedException(Type type, UniqueNameUnit uniqueName, string? propertyName = null)
    : this(type, languageId: null, uniqueName, propertyName)
  {
  }
  public CmsUniqueNameAlreadyUsedException(Type type, LanguageId? languageId, UniqueNameUnit uniqueName, string? propertyName = null)
  : base(BuildMessage(type, languageId, uniqueName.Value, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    LanguageId = languageId?.Value;
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, LanguageId? languageId, string uniqueName, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(LanguageId), languageId, "<null>")
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
  public CmsUniqueNameAlreadyUsedException(LanguageId? languageId, UniqueNameUnit uniqueName, string? propertyName = null)
    : base(typeof(T), languageId, uniqueName, propertyName)
  {
  }
}

using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using ContentType = Logitar.Cms.Core.ContentTypes.ContentType;

namespace Logitar.Cms.Core;

public class AggregateNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified aggregate could not be found.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, Id, PropertyName);

  public AggregateNotFoundException(ContentTypeId contentTypeId, string propertyName)
    : this(typeof(ContentType), contentTypeId.AggregateId, propertyName)
  {
  }
  public AggregateNotFoundException(LanguageId languageId, string propertyName)
    : this(typeof(Language), languageId.AggregateId, propertyName)
  {
  }
  public AggregateNotFoundException(Type type, AggregateId id, string propertyName) : base(BuildMessage(type, id, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentOutOfRangeException(nameof(type));
    }

    TypeName = type.GetNamespaceQualifiedName();
    Id = id.ToGuid();
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, AggregateId id, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(Id), id.ToGuid())
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields;

namespace Logitar.Cms.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
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
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(ContentType contentType, ContentTypeId conflictId)
    : this(conflictId.ToGuid(), contentType.Id.ToGuid(), contentType.UniqueName.Value, nameof(contentType.UniqueName))
  {
  }

  public UniqueNameAlreadyUsedException(FieldType fieldType, FieldTypeId conflictId)
    : this(conflictId.ToGuid(), fieldType.Id.ToGuid(), fieldType.UniqueName.Value, nameof(fieldType.UniqueName))
  {
  }

  public UniqueNameAlreadyUsedException(Guid conflictId, Guid entityId, string uniqueName, string propertyName)
    : base(BuildMessage(conflictId, entityId, uniqueName, propertyName))
  {
    ConflictId = conflictId;
    EntityId = entityId;
    UniqueName = uniqueName;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Guid conflictId, Guid entityId, string uniqueName, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

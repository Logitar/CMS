﻿using Logitar.Cms.Contracts.Errors;
using Logitar.Cms.Core.FieldTypes;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public Guid AggregateId
  {
    get => (Guid)Data[nameof(AggregateId)]!;
    private set => Data[nameof(AggregateId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
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
      PropertyError error = new(this.GetErrorCode(), ErrorMessage, UniqueName, PropertyName);
      error.AddData(nameof(ConflictId), ConflictId.ToString());
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(FieldType fieldType, FieldTypeId conflictId)
    : this(typeof(FieldType), fieldType.Id.AggregateId, conflictId.AggregateId, fieldType.UniqueName, nameof(FieldType.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(Type type, AggregateId aggregateId, AggregateId conflictId, UniqueName uniqueName, string propertyName)
    : base(BuildMessage(type, aggregateId, conflictId, uniqueName, propertyName))
  {
    TypeName = type.GetNamespaceQualifiedName();
    AggregateId = aggregateId.ToGuid();
    ConflictId = conflictId.ToGuid();
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, AggregateId aggregateId, AggregateId conflictId, UniqueName uniqueName, string propertyName)
  {
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
      .AddData(nameof(AggregateId), aggregateId.ToGuid())
      .AddData(nameof(ConflictId), conflictId.ToGuid())
      .AddData(nameof(UniqueName), uniqueName.Value)
      .AddData(nameof(PropertyName), propertyName)
      .Build();
  }
}
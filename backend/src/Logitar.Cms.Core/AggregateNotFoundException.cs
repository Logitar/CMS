using Logitar.Cms.Contracts.Errors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public class AggregateNotFoundException : NotFoundException
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

  public override Error Error => new PropertyError(this.GetErrorCode(), ErrorMessage, PropertyName, Id);

  public AggregateNotFoundException(Type type, AggregateId id, string? propertyName = null)
    : base(BuildMessage(type, id, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of {nameof(AggregateRoot)}.", nameof(type));
    }

    TypeName = type.GetNamespaceQualifiedName();
    Id = id.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, AggregateId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}

public class AggregateNotFoundException<T> : AggregateNotFoundException where T : AggregateRoot
{
  public AggregateNotFoundException(AggregateId id, string? propertyName = null) : base(typeof(T), id, propertyName)
  {
  }
}

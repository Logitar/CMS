using Logitar.EventSourcing;
using System.Text;

namespace Logitar.Cms.Core;

public class AggregateNotFoundException : Exception
{
  public AggregateNotFoundException(Type type, AggregateId id) : base(GetMessage(type, id))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of the {nameof(AggregateRoot)} type.", nameof(type));
    }

    Data["Type"] = type.GetName();
    Data["Id"] = id.ToString();
  }

  private static string GetMessage(Type type, AggregateId id)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate could not be found.");
    message.Append("Type: ").AppendLine(type.GetName());
    message.Append("Id: ").Append(id).AppendLine();

    return message.ToString();
  }
}

public class AggregateNotFoundException<T> : AggregateNotFoundException where T : AggregateRoot
{
  public AggregateNotFoundException(Guid id) : base(typeof(T), new AggregateId(id)) { }
}

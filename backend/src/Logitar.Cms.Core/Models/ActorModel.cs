using Logitar.Cms.Core.Actors;

namespace Logitar.Cms.Core.Models;

public class ActorModel
{
  public static ActorModel System => new(nameof(System));

  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public ActorModel()
  {
  }

  public ActorModel(string displayName)
  {
    DisplayName = displayName;
  }

  public override bool Equals(object? obj) => obj is ActorModel actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder actor = new();
    actor.Append(DisplayName);
    if (EmailAddress != null)
    {
      actor.Append(" <").Append(EmailAddress).Append('>');
    }
    actor.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return actor.ToString();
  }
}

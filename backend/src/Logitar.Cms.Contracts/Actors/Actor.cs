﻿using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Contracts.Actors;

public class Actor
{
  public static Actor System => new(ActorType.System.ToString());

  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public Actor() : this(string.Empty)
  {
  }

  public Actor(ApiKey apiKey) : this(apiKey.DisplayName)
  {
    Id = apiKey.Id;
    Type = ActorType.ApiKey;
  }

  public Actor(User user) : this(user.FullName ?? user.UniqueName)
  {
    Id = user.Id;
    Type = ActorType.User;

    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public Actor(string displayName)
  {
    DisplayName = displayName;
  }

  public override bool Equals(object obj) => obj is Actor actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder s = new();

    s.Append(DisplayName);
    if (EmailAddress != null)
    {
      s.Append(" <").Append(EmailAddress).Append('>');
    }
    s.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');

    return s.ToString();
  }
}

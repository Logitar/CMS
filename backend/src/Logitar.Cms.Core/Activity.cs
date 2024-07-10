using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Users;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

  [JsonIgnore]
  public Actor Actor
  {
    get
    {
      User? user = Context.User;
      if (user != null)
      {
        return new Actor(user);
      }

      ApiKey? apiKey = Context.ApiKey;
      if (apiKey != null)
      {
        return new Actor(apiKey);
      }

      return Actor.System;
    }
  }
  [JsonIgnore]
  public ActorId ActorId => new(Actor.Id);

  public virtual IActivity Anonymize()
  {
    return this;
  }

  public void Contextualize(ActivityContext context)
  {
    if (_context != null)
    {
      throw new InvalidOperationException($"The activity has already been contextualized. You may only call the '{nameof(Contextualize)}' method once.");
    }

    _context = context;
  }
}

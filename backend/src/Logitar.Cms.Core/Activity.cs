using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public abstract record Activity : IActivity
{
  private ActivityContext? _context = null;
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized yet. You must call the {nameof(Contextualize)} method.");

  public virtual Actor Actor
  {
    get
    {
      if (Context.User != null)
      {
        return new Actor(Context.User);
      }

      return Actor.System;
    }
  }
  public virtual ActorId ActorId => new(Actor.Id);

  public virtual void Contextualize(ActivityContext context)
  {
    _context = context;
  }
}

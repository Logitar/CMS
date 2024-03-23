using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core;

public abstract record Request
{
  protected RequestContext? Context { get; set; }

  public Actor Actor => Context?.Actor ?? throw new InvalidOperationException($"The request has not been contextualized yet. You must call the {nameof(Contextualize)} method.");
  public ActorId ActorId => new(Actor.Id);

  public virtual void Contextualize(RequestContext context)
  {
    Context = context;
  }
}

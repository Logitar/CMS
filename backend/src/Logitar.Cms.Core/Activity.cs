using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

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

  public IUserSettings UserSettings => new UserSettings
  {
    UniqueName = Context.Configuration.UniqueNameSettings,
    Password = Context.Configuration.PasswordSettings,
    RequireUniqueEmail = Context.Configuration.RequireUniqueEmail
  };

  public bool RequireUniqueEmail => Context.Configuration.RequireUniqueEmail;

  public virtual void Contextualize(ActivityContext context)
  {
    _context = context;
  }
}

using Logitar.Cms.Contracts.Actors;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Cms.Core;

public abstract record Activity : IActivity
{
  private ActivityContext? _context = null;
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

  public Actor Actor
  {
    get
    {
      if (Context.User != null)
      {
        return new Actor(Context.User);
      }
      else if (Context.ApiKey != null)
      {
        return new Actor(Context.ApiKey);
      }

      return Actor.System;
    }
  }
  public ActorId ActorId => new(Actor.Id);

  private IUserSettings? _userSettings = null;
  public IUserSettings UserSettings
  {
    get
    {
      _userSettings ??= new UserSettings
      {
        UniqueName = Context.Configuration.UniqueNameSettings,
        Password = Context.Configuration.PasswordSettings,
        RequireUniqueEmail = Context.Configuration.RequireUniqueEmail
      };

      return _userSettings;
    }
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

using Logitar.Cms.Contracts.Configurations;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Cms.Core;

public abstract record Activity : IActivity
{
  private ActivityContext? _context = null;
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

  private IUserSettings? _userSettings = null;
  public IUserSettings UserSettings
  {
    get
    {
      if (_userSettings == null)
      {
        Configuration configuration = Context.Configuration;
        _userSettings = new UserSettings
        {
          UniqueName = configuration.UniqueNameSettings,
          Password = configuration.PasswordSettings,
          RequireUniqueEmail = configuration.RequireUniqueEmail
        };
      }

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

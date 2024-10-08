using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using UserSettings = Logitar.Identity.Domain.Settings.UserSettings;

namespace Logitar.Cms.Core;

public abstract record Activity : IActivity
{
  [JsonIgnore]
  private ActivityContext? _context = null;
  [JsonIgnore]
  protected ActivityContext Context => _context ?? throw new InvalidOperationException($"The activity has not been contextualized. You must call the '{nameof(Contextualize)}' method once.");

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

  public ActorId GetActorId()
  {
    ApiKeyModel? apiKey = Context.ApiKey;
    if (apiKey != null)
    {
      return new ActorId(apiKey.Id);
    }

    UserModel? user = Context.User;
    if (user != null)
    {
      return new ActorId(user.Id);
    }

    return new ActorId(Guid.Empty);
  }

  public IUserSettings GetUserSettings()
  {
    ConfigurationModel configuration = Context.Configuration;
    return new UserSettings
    {
      UniqueName = new UniqueNameSettings(configuration.UniqueNameSettings),
      Password = new PasswordSettings(configuration.PasswordSettings),
      RequireUniqueEmail = configuration.RequireUniqueEmail
    };
  }
}

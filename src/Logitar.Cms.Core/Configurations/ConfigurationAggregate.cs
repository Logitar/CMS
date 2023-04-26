using FluentValidation;
using Logitar.Cms.Core.Configurations.Events;
using Logitar.Cms.Core.Configurations.Validators;
using Logitar.EventSourcing;
using System.Security.Cryptography;
using System.Text;

namespace Logitar.Cms.Core.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public static readonly AggregateId GlobalId = new("CONFIGURATION");

  public ConfigurationAggregate(AggregateId id) : base(id) { }

  public ConfigurationAggregate(AggregateId actorId, string? secret = null,
    ReadOnlyLoggingSettings? loggingSettings = null,
    ReadOnlyUsernameSettings? usernameSettings = null,
    ReadOnlyPasswordSettings? passwordSettings = null) : base(GlobalId)
  {
    ConfigurationInitialized e = new(secret ?? GenerateSecret(), loggingSettings ?? new(),
      usernameSettings ?? new(), passwordSettings ?? new())
    {
      ActorId = actorId
    };
    new ConfigurationInitializedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(ConfigurationInitialized e)
  {
    Secret = e.Secret;

    LoggingSettings = e.LoggingSettings;

    UsernameSettings = e.UsernameSettings;
    PasswordSettings = e.PasswordSettings;
  }

  public string Secret { get; private set; } = string.Empty;

  public ReadOnlyLoggingSettings LoggingSettings { get; private set; } = new();

  public ReadOnlyUsernameSettings UsernameSettings { get; private set; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; private set; } = new();

  internal static string GenerateSecret(int length = 256 / 8)
  {
    while (true)
    {
      /* In the ASCII table, there are 94 characters between 33 '!' and 126 '~' (126 - 33 + 1 = 94).
       * Since there are a total of 256 possibilities, by dividing per 94 and adding a 10% margin we
       * generate just a little more bytes than required, obtaining the factor 3. */
      byte[] bytes = RandomNumberGenerator.GetBytes(length * 3);

      List<byte> secret = new(capacity: length);
      for (int i = 0; i < bytes.Length; i++)
      {
        byte @byte = bytes[i];
        if (@byte >= 33 && @byte <= 126)
        {
          secret.Add(@byte);

          if (secret.Count == length)
          {
            return Encoding.ASCII.GetString(secret.ToArray());
          }
        }
      }
    }
  }
}

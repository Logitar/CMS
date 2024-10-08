using Bogus;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Actors;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Users;
using Logitar.Cms.Core.Configurations;

namespace Logitar.Cms.Core;

internal static class ActivityExtensions
{
  private static readonly Faker _faker = new();

  public static void Contextualize(this Activity activity, Faker? faker = null)
  {
    faker ??= _faker;

    DateTime now = DateTime.UtcNow;
    UserModel user = new()
    {
      Id = Guid.NewGuid(),
      Version = 1,
      CreatedOn = now,
      UpdatedOn = now,
      Username = faker.Person.UserName,
      HasPassword = true,
      PasswordChangedOn = now,
      Email = new EmailModel(faker.Person.Email)
      {
        IsVerified = true,
        VerifiedOn = now
      },
      IsConfirmed = true,
      FirstName = faker.Person.FirstName,
      LastName = faker.Person.LastName,
      FullName = faker.Person.FullName,
      Birthdate = faker.Person.DateOfBirth,
      Gender = faker.Person.Gender.ToString(),
      Locale = new LocaleModel(faker.Locale),
      TimeZone = "America/Montreal",
      Picture = faker.Person.Avatar,
      Website = $"https://www.{faker.Person.Website}",
      AuthenticatedOn = now
    };

    Actor actor = new(user.FullName)
    {
      Id = user.Id,
      Type = ActorType.User,
      EmailAddress = user.Email.Address,
      PictureUrl = user.Picture
    };
    user.CreatedBy = actor;
    user.UpdatedBy = actor;
    user.PasswordChangedBy = actor;
    user.Email.VerifiedBy = actor;

    ConfigurationModel configuration = new()
    {
      Secret = JwtSecret.Generate().Value,
      UniqueNameSettings = new UniqueNameSettingsModel
      {
        AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
      },
      PasswordSettings = new PasswordSettingsModel
      {
        RequiredLength = 8,
        RequiredUniqueChars = 8,
        RequireNonAlphanumeric = true,
        RequireLowercase = true,
        RequireUppercase = true,
        RequireDigit = true,
        HashingStrategy = "PBKDF2"
      },
      RequireUniqueEmail = true,
      LoggingSettings = new LoggingSettingsModel
      {
        Extent = LoggingExtent.ActivityOnly
      }
    };

    ActivityContext context = new(configuration, ApiKey: null, Session: null, user);
    activity.Contextualize(context);
  }
}

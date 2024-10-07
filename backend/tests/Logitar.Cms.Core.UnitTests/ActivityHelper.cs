using Bogus;
using Logitar.Cms.Contracts;
using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Contracts.Users;
using Logitar.Security.Cryptography;

namespace Logitar.Cms.Core;

internal static class ActivityHelper
{
  private static readonly Faker _faker = new();

  public static void Contextualize(this IActivity activity, Person? person = null)
  {
    person ??= _faker.Person;

    Configuration configuration = new(RandomStringGenerator.GetString(32))
    {
      RequireUniqueEmail = true
    };
    User user = new(person.UserName)
    {
      Id = Guid.NewGuid(),
      Email = new Email(person.Email),
      FirstName = person.FirstName,
      LastName = person.LastName,
      FullName = person.FullName,
      Birthdate = person.DateOfBirth,
      Gender = person.Gender.ToString().ToLower(),
      Locale = new Locale("en"),
      Picture = person.Avatar,
      Website = $"https://www.{person.Website}"
    };
    ActivityContext context = new(configuration, ApiKey: null, Session: null, user);

    activity.Contextualize(context);
  }
}

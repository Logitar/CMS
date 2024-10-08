using Bogus;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;

namespace Logitar.Cms.Core.Builders;

internal class UserBuilder
{
  private readonly Faker _faker;

  public string? Password { get; private set; }

  public UserBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public UserBuilder WithPassword(string password)
  {
    Password = password;
    return this;
  }

  public UserAggregate Build()
  {
    UserId id = UserId.NewId();
    ActorId actorId = new(id.Value);
    UniqueNameUnit uniqueName = new(new UniqueNameSettings(), _faker.Person.UserName);
    UserAggregate user = new(uniqueName, tenantId: null, actorId, id);

    if (Password != null)
    {
      user.SetPassword(new PasswordMock(Password), actorId);
    }

    return user;
  }
}

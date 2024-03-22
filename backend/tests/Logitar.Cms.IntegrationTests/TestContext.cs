using Bogus;
using Logitar.Cms.Contracts.Actors;

namespace Logitar.Cms;

internal class TestContext
{
  public Actor Actor { get; }

  public TestContext(Actor actor)
  {
    Actor = actor;
  }

  public static TestContext Random(Faker faker)
  {
    Actor actor = new(faker.Person.FullName)
    {
      Id = Guid.NewGuid(),
      Type = ActorType.User,
      EmailAddress = faker.Person.Email,
      PictureUrl = faker.Person.Avatar
    };

    return new TestContext(actor);
  }
}

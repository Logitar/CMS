using Bogus;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class SessionAggregateTests
{
  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly Faker _faker = new();

  private readonly ConfigurationAggregate _configuration;
  private readonly UserAggregate _user;

  public SessionAggregateTests()
  {
    _configuration = new(actorId: _actorId);
    _user = new(_actorId, _configuration, "admin");
  }

  [Theory]
  [InlineData("9245822d-5246-4a56-9154-db4786322cc5")]
  public void When_it_is_constructed_with_id_Then_it_has_correct_id(string id)
  {
    AggregateId aggregateId = new(id);
    UserAggregate user = new(aggregateId);

    Assert.Equal(aggregateId, user.Id);
  }

  [Fact]
  public void When_it_is_constructed_with_invalid_arguments_Then_ValidationException_is_thrown()
  {
    string ipAddress = _faker.Random.String(byte.MaxValue + 1);

    var exception = Assert.Throws<FluentValidation.ValidationException>(()
      => new SessionAggregate(_user, signedInOn: DateTime.UtcNow, ipAddress: ipAddress));

    Assert.Equal("IpAddress", exception.Errors.Single().PropertyName);
  }

  [Fact]
  public void When_it_is_constructed_with_valid_arguments_Then_it_is_created()
  {
    DateTime signedInOn = DateTime.UtcNow.AddHours(-1);
    bool isPersistent = true;
    string ipAddress = "   ::1 ";
    string additionalInformation = " {\"User-Agent\":\"Chrome\"}   ";

    SessionAggregate session = new(_user, signedInOn, isPersistent, ipAddress, additionalInformation);

    Assert.Equal(signedInOn, session.CreatedOn);
    Assert.Equal(signedInOn, session.UpdatedOn);
    Assert.Equal(_user.Id, session.UserId);
    Assert.Equal(isPersistent, session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Equal(ipAddress.Trim(), session.IpAddress);
    Assert.Equal(additionalInformation.Trim(), session.AdditionalInformation);

    Assert.True(session.RefreshToken.HasValue);
    Assert.Equal(session.Id.ToGuid(), session.RefreshToken.Value.Id);
    Assert.Equal(32, session.RefreshToken.Value.Secret.Length);
  }

  [Fact]
  public void When_it_is_not_persistent_Then_it_does_not_have_a_refresh_token()
  {
    SessionAggregate session = new(_user, signedInOn: DateTime.UtcNow);

    Assert.False(session.IsPersistent);
    Assert.False(session.RefreshToken.HasValue);
  }
}

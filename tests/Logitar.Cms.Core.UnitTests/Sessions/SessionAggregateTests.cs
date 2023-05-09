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

  [Fact]
  public void When_it_is_active_Then_signing_it_out_should_deactive_it()
  {
    SessionAggregate session = new(_user);
    Assert.True(session.IsActive);

    session.SignOut(_actorId);
    Assert.False(session.IsActive);
  }

  [Theory]
  [InlineData("9245822d-5246-4a56-9154-db4786322cc5")]
  public void When_it_is_constructed_with_id_Then_it_has_correct_id(string id)
  {
    AggregateId aggregateId = new(id);
    SessionAggregate session = new(aggregateId);

    Assert.Equal(aggregateId, session.Id);
  }

  [Fact]
  public void When_it_is_constructed_with_invalid_arguments_Then_ValidationException_is_thrown()
  {
    string ipAddress = _faker.Random.String(byte.MaxValue + 1);

    var exception = Assert.Throws<FluentValidation.ValidationException>(()
      => new SessionAggregate(_user, ipAddress: ipAddress));

    Assert.Equal("IpAddress", exception.Errors.Single().PropertyName);
  }

  [Fact]
  public void When_it_is_constructed_with_valid_arguments_Then_it_is_created()
  {
    DateTime signedInOn = DateTime.UtcNow.AddHours(-1);
    bool isPersistent = true;
    string ipAddress = "   ::1 ";
    string additionalInformation = " {\"User-Agent\":\"Chrome\"}   ";

    SessionAggregate session = new(_user, isPersistent, signedInOn, ipAddress, additionalInformation);

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
  public void When_it_is_not_active_Then_refreshing_it_should_throw_SessionIsNotActiveException()
  {
    SessionAggregate session = new(_user, isPersistent: true);
    session.SignOut(_actorId);
    Assert.True(session.RefreshToken.HasValue);
    Assert.False(session.IsActive);

    Assert.Throws<SessionIsNotActiveException>(() => session.Refresh(session.RefreshToken.Value.Secret));
  }

  [Fact]
  public void When_it_is_not_active_Then_signing_it_out_should_throw_SessionIsNotActiveException()
  {
    SessionAggregate session = new(_user);
    session.SignOut(_actorId);
    Assert.False(session.IsActive);

    Assert.Throws<SessionIsNotActiveException>(() => session.SignOut(_actorId));
  }

  [Fact]
  public void When_it_is_not_persistent_Then_it_does_not_have_a_refresh_token()
  {
    SessionAggregate session = new(_user);

    Assert.False(session.IsPersistent);
    Assert.False(session.RefreshToken.HasValue);
  }

  [Fact]
  public void When_it_is_not_persistent_Then_refreshing_it_should_throw_InvalidCredentialsException()
  {
    SessionAggregate session = new(_user, isPersistent: false);
    Assert.Throws<InvalidCredentialsException>(() => session.Refresh(secretBytes: Array.Empty<byte>()));
  }

  [Fact]
  public void When_it_is_refreshed_using_incorrect_secret_Then_InvalidCredentialsException_is_thrown()
  {
    SessionAggregate session = new(_user, isPersistent: true);
    Assert.True(session.RefreshToken.HasValue);

    byte[] incorrectSecret = session.RefreshToken.Value.Secret.Skip(1).ToArray();
    Assert.Throws<InvalidCredentialsException>(() => session.Refresh(incorrectSecret));
  }

  [Fact]
  public void When_it_is_refreshed_using_invalid_parameters_Then_FluentValidation_is_thrown()
  {
    string ipAddress = _faker.Random.String(byte.MaxValue + 1);

    SessionAggregate session = new(_user, isPersistent: true);
    Assert.True(session.RefreshToken.HasValue);

    var exception = Assert.Throws<FluentValidation.ValidationException>(()
      => session.Refresh(session.RefreshToken.Value.Secret, ipAddress));

    Assert.Equal("IpAddress", exception.Errors.Single().PropertyName);
  }

  [Fact]
  public void When_it_is_refreshed_using_valid_parameters_Then_it_is_refreshed()
  {
    SessionAggregate session = new(_user, isPersistent: true);

    string ipAddress = "   ::1 ";
    string additionalInformation = " {\"User-Agent\":\"Chrome\"}   ";
    Assert.True(session.RefreshToken.HasValue);
    byte[] oldSecret = session.RefreshToken.Value.Secret;
    session.Refresh(session.RefreshToken.Value.Secret, ipAddress, additionalInformation);

    Assert.Equal(ipAddress.Trim(), session.IpAddress);
    Assert.Equal(additionalInformation.Trim(), session.AdditionalInformation);

    Assert.True(session.RefreshToken.HasValue);
    Assert.Equal(session.Id.ToGuid(), session.RefreshToken.Value.Id);
    Assert.NotEqual(oldSecret, session.RefreshToken.Value.Secret);
  }
}

using Bogus;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Users.Events;
using Logitar.EventSourcing;
using System.Globalization;

namespace Logitar.Cms.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserAggregateTests
{
  private const string Username = "admin";

  private readonly AggregateId _actorId = AggregateId.NewId();
  private readonly Faker _faker = new();

  private readonly ConfigurationAggregate _configuration;

  public UserAggregateTests()
  {
    _configuration = new(actorId: _actorId);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void When_email_is_modified_Then_it_has_correct_verification_action(bool isVerified)
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    user.SetEmail(new ReadOnlyEmail(_faker.Person.Email));

    ReadOnlyEmail email = new(_faker.Person.Email.Replace("@", "+1@"), isVerified);
    user.SetEmail(email);

    Assert.Same(email, user.Email);

    EmailChanged e = (EmailChanged)user.Changes.Where(e => e is EmailChanged).Last();
    if (isVerified)
    {
      Assert.Equal(VerificationAction.Verify, e.VerificationAction);
    }
    else
    {
      Assert.Equal(VerificationAction.Unverify, e.VerificationAction);
    }
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void When_email_is_not_modified_Then_it_has_correct_verification_action(bool isVerified)
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    user.SetEmail(new ReadOnlyEmail(_faker.Person.Email));

    ReadOnlyEmail email = new(_faker.Person.Email, isVerified);
    user.SetEmail(email);

    Assert.Same(email, user.Email);

    EmailChanged e = (EmailChanged)user.Changes.Where(e => e is EmailChanged).Last();
    if (isVerified)
    {
      Assert.Equal(VerificationAction.Verify, e.VerificationAction);
    }
    else
    {
      Assert.Null(e.VerificationAction);
    }
  }

  [Theory]
  [InlineData(Username)]
  public void When_it_is_constructed_with_id_Then_it_has_correct_id(string id)
  {
    AggregateId aggregateId = new(id);
    UserAggregate user = new(aggregateId);

    Assert.Equal(aggregateId, user.Id);
  }

  [Fact]
  public void When_it_is_constructed_with_invalid_parameters_Then_ValidationException_is_thrown()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new UserAggregate(_actorId,
      _configuration, username: "test!", locale: CultureInfo.InvariantCulture));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.PropertyName == "Username");
    Assert.Contains(exception.Errors, e => e.PropertyName == "Locale");
  }

  [Theory]
  [InlineData(null)]
  [InlineData(Username)]
  public void When_it_is_constructed_with_valid_parameters_Then_it_is_created(string? userId = null)
  {
    AggregateId actorId = AggregateId.NewId();
    AggregateId? id = userId == null ? null : new(userId);

    string username = $" {_faker.Person.UserName}   ";
    string firstName = $" {_faker.Person.FirstName}   ";
    string lastName = $" {_faker.Person.LastName}   ";
    string? fullName = UserAggregate.GetFullName(firstName, lastName);
    CultureInfo locale = CultureInfo.GetCultureInfo("fr-CA");
    Uri picture = new("https://www.test.com/assets/img/admin.jpg");

    UserAggregate user = new(actorId, _configuration, username, firstName, lastName, locale, picture, id);

    Assert.Equal(actorId, user.CreatedById);
    Assert.Equal(actorId, user.UpdatedById);
    Assert.Equal(username.Trim(), user.Username);
    Assert.Equal(firstName.Trim(), user.FirstName);
    Assert.Equal(lastName.Trim(), user.LastName);
    Assert.Equal(fullName, user.FullName);
    Assert.Equal(username.Trim(), user.Username);
    Assert.Equal(locale, user.Locale);
    Assert.Equal(picture, user.Picture);

    if (userId != null)
    {
      Assert.Equal(userId, user.Id.Value);
    }
  }

  [Fact]
  public void When_it_has_a_full_name_Then_string_representation_displays_full_name_and_username()
  {
    UserAggregate user = new(_actorId, _configuration, Username, _faker.Person.FirstName, _faker.Person.LastName);
    string s = $"{user.FullName} ({user.Username})";

    Assert.Equal(s, user.ToString().Split('|').First().Trim());
  }

  [Fact]
  public void When_it_has_no_full_name_Then_string_representation_only_displays_username()
  {
    UserAggregate user = new(_actorId, _configuration, Username);

    Assert.Equal(user.Username, user.ToString().Split('|').First().Trim());
  }

  [Fact]
  public void When_new_password_is_not_valid_Then_ValidationException_is_thrown()
  {
    UserAggregate user = new(_actorId, _configuration, Username);

    var exception = Assert.Throws<FluentValidation.ValidationException>(()
      => user.ChangePassword(_configuration, password: "Aa!1"));

    Assert.Equal("PasswordTooShort", exception.Errors.Single().ErrorCode);
  }

  [Fact]
  public void When_new_password_is_valid_Then_it_is_changed()
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    Assert.False(user.HasPassword);

    user.ChangePassword(_configuration, "P@s$W0rD");
    Assert.True(user.HasPassword);
  }

  [Fact]
  public void When_setting_an_invalid_email_Then_ValidationException_is_thrown()
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    ReadOnlyEmail email = new("aa@@bb..cc");

    var exception = Assert.Throws<FluentValidation.ValidationException>(() => user.SetEmail(email));

    Assert.Equal("Email.Address", exception.Errors.Single().PropertyName);
  }

  [Fact]
  public void When_setting_null_email_Then_email_is_null_and_unverified()
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    user.SetEmail(new ReadOnlyEmail(_faker.Person.Email));
    Assert.NotNull(user.Email);

    user.SetEmail(email: null);
    Assert.Null(user.Email);

    EmailChanged e = (EmailChanged)user.Changes.Where(e => e is EmailChanged).Last();
    Assert.Equal(VerificationAction.Unverify, e.VerificationAction);
  }

  [Theory]
  [InlineData("P@s$W0rD")]
  [InlineData("P@s$W0rD", true, "::1", "{\"User-Agent\":\"Chrome\"}")]
  public void When_signing_in_with_a_correct_password_Then_it_returns_a_SessionAggregate(string password,
    bool remember = false, string? ipAddress = null, string? additionalInformation = null)
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    user.ChangePassword(_configuration, password);

    SessionAggregate session = user.SignIn(password, remember, ipAddress, additionalInformation);

    Assert.Equal(user.Id, session.UserId);
    Assert.Equal(user.Changes.Single(c => c is UserSignedIn).OccurredOn, session.CreatedOn);
    Assert.Equal(remember, session.IsPersistent);
    Assert.Equal(ipAddress, session.IpAddress);
    Assert.Equal(additionalInformation, session.AdditionalInformation);
  }

  [Theory]
  [InlineData("P@s$W0rD")]
  public void When_signing_in_with_an_incorrect_password_Then_InvalidCredentialsException_is_thrown(string password)
  {
    UserAggregate user = new(_actorId, _configuration, Username);
    user.ChangePassword(_configuration, password);

    Assert.Throws<InvalidCredentialsException>(() => user.SignIn(password[..^1]));
  }

  [Fact]
  public void When_signing_in_without_a_password_Then_InvalidCredentialsException_is_thrown()
  {
    UserAggregate user = new(_actorId, _configuration, Username);

    Assert.Throws<InvalidCredentialsException>(() => user.SignIn(password: string.Empty));
  }

  [Fact]
  public void When_there_are_no_name_Then_GetFullName_returns_null()
  {
    Assert.Null(UserAggregate.GetFullName());
  }

  [Fact]
  public void When_there_are_no_valid_name_Then_GetFullName_returns_null()
  {
    Assert.Null(UserAggregate.GetFullName(null, string.Empty, "    "));
  }

  [Fact]
  public void When_there_are_valid_names_Then_GetFullName_returns_trimmed_full_name()
  {
    string? fullName = UserAggregate.GetFullName("Charles-Antoine  ", null, "Paget  Merlot", string.Empty, "  Boucher  ", "  ", "   Lessard");
    Assert.Equal("Charles-Antoine Paget Merlot Boucher Lessard", fullName);
  }
}

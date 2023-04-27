using Bogus;
using Logitar.Cms.Contracts.Users;
using System.Security.Claims;

namespace Logitar.Cms.Core.Claims;

[Trait(Traits.Category, Categories.Unit)]
public class ClaimExtensionsTests
{
  private readonly Faker _faker = new();

  [Theory]
  [InlineData("2023-04-27T03:48:00Z", Rfc7519ClaimTypes.AuthenticationTime)]
  public void It_should_return_the_correct_date_time_claim(string dateTime, string type)
  {
    DateTime moment = DateTime.Parse(dateTime);
    Claim claim = moment.GetClaim(type);

    Assert.Equal(new DateTimeOffset(moment).ToUnixTimeSeconds().ToString(), claim.Value);
    Assert.Equal(type, claim.Type);
    Assert.Equal(ClaimValueTypes.Integer64, claim.ValueType);
  }

  [Fact]
  public void It_should_return_the_correct_user_claims_identity()
  {
    DateTime now = DateTime.UtcNow;
    User user = new()
    {
      Id = Guid.NewGuid(),
      Username = _faker.Person.UserName,
      SignedInOn = now,
      Email = new Email
      {
        Address = _faker.Person.Email,
        IsVerified = true
      },
      FirstName = _faker.Person.FirstName,
      LastName = _faker.Person.LastName,
      FullName = _faker.Person.FullName,
      Locale = "fr-CA",
      Picture = "www.test.com/assets/img/admin.jpg",
      UpdatedOn = now.AddMinutes(-1)
    };

    ClaimsIdentity identity = user.GetClaimsIdentity();
    Assert.Null(identity.AuthenticationType);

    foreach (Claim claim in identity.Claims)
    {
      switch (claim.Type)
      {
        case Rfc7519ClaimTypes.Subject:
          Assert.Equal(user.Id.ToString(), claim.Value);
          break;
        case Rfc7519ClaimTypes.Username:
          Assert.Equal(user.Username, claim.Value);
          break;
        case Rfc7519ClaimTypes.UpdatedOn:
          Assert.Equal(new DateTimeOffset(user.UpdatedOn).ToUnixTimeSeconds().ToString(), claim.Value);
          Assert.Equal(ClaimValueTypes.Integer64, claim.ValueType);
          break;
        case Rfc7519ClaimTypes.EmailAddress:
          Assert.Equal(user.Email.Address, claim.Value);
          break;
        case Rfc7519ClaimTypes.IsEmailVerified:
          Assert.Equal(user.Email.IsVerified.ToString().ToLower(), claim.Value);
          Assert.Equal(ClaimValueTypes.Boolean, claim.ValueType);
          break;
        case Rfc7519ClaimTypes.FullName:
          Assert.Equal(user.FullName, claim.Value);
          break;
        case Rfc7519ClaimTypes.FirstName:
          Assert.Equal(user.FirstName, claim.Value);
          break;
        case Rfc7519ClaimTypes.LastName:
          Assert.Equal(user.LastName, claim.Value);
          break;
        case Rfc7519ClaimTypes.Locale:
          Assert.Equal(user.Locale, claim.Value);
          break;
        case Rfc7519ClaimTypes.Picture:
          Assert.Equal(user.Picture, claim.Value);
          break;
        case Rfc7519ClaimTypes.AuthenticationTime:
          Assert.Equal(new DateTimeOffset(user.SignedInOn.Value).ToUnixTimeSeconds().ToString(), claim.Value);
          Assert.Equal(ClaimValueTypes.Integer64, claim.ValueType);
          break;
        default:
          Assert.Fail($"The claim '{claim.Type}' should not be present in the claims identity.");
          break;
      }
    }
  }

  [Theory]
  [InlineData("Bearer")]
  public void When_an_authentication_type_is_specified_Then_it_is_assigned_to_the_user_claims_identity(string authenticationType)
  {
    User user = new();
    ClaimsIdentity identity = user.GetClaimsIdentity(authenticationType);

    Assert.Equal(authenticationType, identity.AuthenticationType);
  }
}

using Logitar.EventSourcing;
using System.Security.Cryptography;

namespace Logitar.Cms.Core.Sessions;

[Trait(Traits.Category, Categories.Unit)]
public class RefreshTokenTests
{
  private readonly Guid _id = Guid.NewGuid();
  private readonly byte[] _secret = RandomNumberGenerator.GetBytes(32);

  private readonly string _s;

  public RefreshTokenTests()
  {
    _s = Convert.ToBase64String(_id.ToByteArray().Concat(_secret).ToArray()).ToUriSafeBase64();
  }

  [Fact]
  public void It_should_return_the_correct_hash_code()
  {
    int hashCode = HashCode.Combine(_id, _secret);
    Assert.Equal(hashCode, new RefreshToken(_id, _secret).GetHashCode());
  }

  [Fact]
  public void It_should_return_the_correct_string_representation()
  {
    string s = Convert.ToBase64String(_id.ToByteArray().Concat(_secret).ToArray()).ToUriSafeBase64();
    Assert.Equal(s, new RefreshToken(_id, _secret).ToString());
  }

  [Fact]
  public void When_compared_with_a_different_type_Then_they_are_not_equal()
  {
    Assert.False(new RefreshToken(_id, _secret).Equals(_s));
  }

  [Fact]
  public void When_comparing_different_refresh_tokens_Then_they_are_not_equal()
  {
    RefreshToken token = new(_id, _secret);
    RefreshToken other = new(Guid.NewGuid(), RandomNumberGenerator.GetBytes(32));

    Assert.False(token == other);
    Assert.True(token != other);

    Assert.False(token.Equals(other));
  }

  [Fact]
  public void When_comparing_same_refresh_tokens_Then_they_are_equal()
  {
    RefreshToken token = new(_id, _secret);
    RefreshToken other = new(_id, _secret);

    Assert.True(token == other);
    Assert.False(token != other);

    Assert.True(token.Equals(other));
  }

  [Fact]
  public void When_constructed_with_parameters_Then_it_has_valid_properties()
  {
    RefreshToken token = new(_id, _secret);
    Assert.Equal(_id, token.Id);
    Assert.Equal(_secret, token.Secret);
  }

  [Fact]
  public void When_id_is_empty_Then_ArgumentException_is_thrown()
  {
    var exception = Assert.Throws<ArgumentException>(() => new RefreshToken(Guid.Empty, _secret));
    Assert.Equal("id", exception.ParamName);
  }

  [Fact]
  public void When_parsing_using_invalid_string_representation_Then_it_is_not_parsed()
  {
    Assert.False(RefreshToken.TryParse("test", out RefreshToken token));
    Assert.Equal(default, token);
  }

  [Fact]
  public void When_parsing_using_valid_string_representation_Then_it_is_parsed()
  {
    Assert.True(RefreshToken.TryParse(_s, out RefreshToken token));
    Assert.Equal(_id, token.Id);
    Assert.Equal(_secret, token.Secret);
  }

  [Fact]
  public void When_parsing_using_string_representation_Then_it_is_correctly_parsed()
  {
    RefreshToken token = RefreshToken.Parse(_s);
    Assert.Equal(_id, token.Id);
    Assert.Equal(_secret, token.Secret);
  }

  [Fact]
  public void When_secret_is_empty_Then_ArgumentException_is_thrown()
  {
    var exception = Assert.Throws<ArgumentException>(() => new RefreshToken(_id, Array.Empty<byte>()));
    Assert.Equal("secret", exception.ParamName);
  }
}

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Reflection;

namespace Logitar.Cms.Core.Security;

[Trait(Traits.Category, Categories.Unit)]
public class Pbkdf2Tests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly Pbkdf2 _pbkdf2 = new(PasswordString);

  [Fact]
  public void It_should_return_the_correct_hash_code()
  {
    int hashCode = HashCode.Combine(GetAlgorithm(), GetIterationCount(), Convert.ToBase64String(GetSalt()), Convert.ToBase64String(GetHash()));
    Assert.Equal(hashCode, _pbkdf2.GetHashCode());
  }

  [Fact]
  public void It_should_return_the_correct_string_representation()
  {
    string s = string.Join(':', GetAlgorithm(), GetIterationCount(), Convert.ToBase64String(GetSalt()), Convert.ToBase64String(GetHash()));
    Assert.Equal(s, _pbkdf2.ToString());
  }

  [Fact]
  public void When_instances_have_different_properties_Then_they_are_equal()
  {
    Assert.False(_pbkdf2.Equals(new Pbkdf2(PasswordString[..^1])));
  }

  [Fact]
  public void When_instances_have_the_same_properties_Then_they_are_equal()
  {
    Assert.True(_pbkdf2.Equals(_pbkdf2));
  }

  [Fact]
  public void When_it_is_created_from_a_password_Then_it_has_correct_values()
  {
    KeyDerivationPrf algorithm = GetAlgorithm();
    Assert.Equal(KeyDerivationPrf.HMACSHA256, algorithm);

    int iterationCount = GetIterationCount();
    Assert.Equal(100000, iterationCount);

    byte[] salt = GetSalt();
    Assert.Equal(32, salt.Length);

    byte[] hash = KeyDerivation.Pbkdf2(PasswordString, salt, algorithm, iterationCount, salt.Length);
    Assert.Equal(hash, GetHash());
  }

  [Fact]
  public void When_matching_correct_password_Then_it_returns_true()
  {
    Assert.True(_pbkdf2.IsMatch(PasswordString));
  }

  [Fact]
  public void When_matching_incorrect_password_Then_it_returns_false()
  {
    Assert.False(_pbkdf2.IsMatch(PasswordString[..^1]));
  }

  [Fact]
  public void When_string_representation_is_not_valid_Then_ArgumentException_is_thrown()
  {
    var exception = Assert.Throws<ArgumentException>(() => Pbkdf2.Parse("test"));
    Assert.Equal("s", exception.ParamName);
  }

  [Fact]
  public void When_string_representation_is_not_valid_Then_it_cannot_be_parsed()
  {
    Assert.False(Pbkdf2.TryParse("test", out Pbkdf2? pbkdf2));
    Assert.Null(pbkdf2);
  }

  [Fact]
  public void When_string_representation_is_valid_Then_it_can_be_parsed()
  {
    string s = string.Join(':', GetAlgorithm(), GetIterationCount(), Convert.ToBase64String(GetSalt()), Convert.ToBase64String(GetHash()));
    Assert.True(Pbkdf2.TryParse(s, out Pbkdf2? pbkdf2));
    Assert.NotNull(pbkdf2);

    AssertEqual(_pbkdf2, pbkdf2);
  }

  [Fact]
  public void When_string_representation_is_valid_Then_it_is_parsed()
  {
    string s = string.Join(':', GetAlgorithm(), GetIterationCount(), Convert.ToBase64String(GetSalt()), Convert.ToBase64String(GetHash()));
    Pbkdf2 pbkdf2 = Pbkdf2.Parse(s);

    AssertEqual(_pbkdf2, pbkdf2);
  }

  private void AssertEqual(Pbkdf2 expected, Pbkdf2 actual)
  {
    Assert.Equal(GetAlgorithm(expected), GetAlgorithm(actual));
    Assert.Equal(GetIterationCount(expected), GetIterationCount(actual));
    Assert.Equal(GetSalt(expected), GetSalt(actual));
    Assert.Equal(GetHash(expected), GetHash(actual));
  }

  private KeyDerivationPrf GetAlgorithm(Pbkdf2? pbkdf2 = null)
    => (KeyDerivationPrf?)GetField<Pbkdf2>("_algorithm").GetValue(pbkdf2 ?? _pbkdf2)
      ?? throw new InvalidOperationException("The '_algorithm' field value could not be resolved.");
  private int GetIterationCount(Pbkdf2? pbkdf2 = null)
    => (int?)GetField<Pbkdf2>("_iterationCount").GetValue(pbkdf2 ?? _pbkdf2)
      ?? throw new InvalidOperationException("The '_iterationCount' field value could not be resolved.");
  private byte[] GetSalt(Pbkdf2? pbkdf2 = null)
    => (byte[]?)GetField<Pbkdf2>("_salt").GetValue(pbkdf2 ?? _pbkdf2)
      ?? throw new InvalidOperationException("The '_salt' field value could not be resolved.");
  private byte[] GetHash(Pbkdf2? pbkdf2 = null)
    => (byte[]?)GetField<Pbkdf2>("_hash").GetValue(pbkdf2 ?? _pbkdf2)
      ?? throw new InvalidOperationException("The '_hash' field value could not be resolved.");
  private static FieldInfo GetField<T>(string name) => typeof(T).GetTypeInfo()
    .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
    ?? throw new InvalidOperationException($"The '{name}' field information could not be resolved.");
}

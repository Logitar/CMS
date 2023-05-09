using Logitar.Cms.Contracts.Users;

namespace Logitar.Cms.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class ReadOnlyEmailTests
{
  [Theory]
  [InlineData("info@test.com", false)]
  [InlineData("info@test.com ", true)]
  public void It_should_return_a_read_only_email(string address, bool verify)
  {
    EmailInput input = new()
    {
      Address = address,
      Verify = verify
    };

    ReadOnlyEmail? email = ReadOnlyEmail.From(input);
    Assert.NotNull(email);
    Assert.Equal(address.Trim(), email.Address);
    Assert.Equal(verify, email.IsVerified);
  }

  [Fact]
  public void When_it_is_a_null_email_input_Then_it_returns_null()
  {
    Assert.Null(ReadOnlyEmail.From(input: null));
  }
}

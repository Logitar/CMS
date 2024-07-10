using Logitar.Cms.Contracts.Sessions;

namespace Logitar.Cms.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignInSessionCommandHandlerTests : IntegrationTests
{
  public SignInSessionCommandHandlerTests() : base()
  {
  }

  [Fact(DisplayName = "It should create a new session.")]
  public async Task It_should_create_a_new_session()
  {
    SignInSessionPayload payload = new(UsernameString, PasswordString, isPersistent: true);
    SignInSessionCommand command = new(payload);
    Session session = await Pipeline.ExecuteAsync(command);

    Assert.NotEqual(default, session.Id);
    Assert.Equal(1, session.Version);
    Assert.NotEqual(default, session.CreatedOn);
    Assert.Equal(session.User.Id, session.CreatedBy.Id);
    Assert.Equal(session.CreatedBy, session.UpdatedBy);
    Assert.Equal(session.CreatedOn, session.UpdatedOn);

    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Empty(session.CustomAttributes);
    Assert.Equal(UsernameString, session.User.UniqueName);
  }
}

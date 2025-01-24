using Logitar.Identity.Core.Roles;
using Moq;

namespace Logitar.Cms.Core.Roles.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class FindRolesQueryHandlerTests
{
  //private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRoleRepository> _roleRepository = new();

  private readonly FindRolesQueryHandler _handler;

  public FindRolesQueryHandlerTests()
  {
    _handler = new(_roleRepository.Object);
  }

  // TODO(fpion): implement
}

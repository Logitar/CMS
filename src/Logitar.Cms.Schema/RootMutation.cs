using GraphQL.Types;

namespace Logitar.Cms.Schema;

internal class RootMutation : ObjectGraphType
{
  public RootMutation()
  {
    Name = nameof(RootMutation);
  }
}

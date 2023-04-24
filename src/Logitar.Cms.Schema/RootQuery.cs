using GraphQL.Types;
using Logitar.Cms.Schema.Resources;

namespace Logitar.Cms.Schema;

internal class RootQuery : ObjectGraphType
{
  public RootQuery()
  {
    Name = nameof(RootQuery);

    ResourceQueries.Register(this);
  }
}

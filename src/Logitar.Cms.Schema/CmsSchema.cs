using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Schema;

public class CmsSchema : GraphQL.Types.Schema
{
  public CmsSchema(IServiceProvider serviceProvider) : base(serviceProvider)
  {
    Query = serviceProvider.GetRequiredService<RootQuery>();
  }
}

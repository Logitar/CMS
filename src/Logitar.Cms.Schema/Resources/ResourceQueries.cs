using GraphQL.Types;
using Logitar.Cms.Contracts.Resources;
using Logitar.Cms.Schema.Extensions;

namespace Logitar.Cms.Schema.Resources
{
  internal static class ResourceQueries
  {
    internal static void Register(RootQuery root)
    {
      root.Field<ListGraphType<LocaleGraphType>>("locales")
        .Description("Retrieves a cached list of supported locales by this application.")
        .Resolve(context => context.GetRequiredService<IResourceService, object?>().GetLocales());
    }
  }
}

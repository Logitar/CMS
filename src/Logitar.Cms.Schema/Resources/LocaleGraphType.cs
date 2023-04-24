using GraphQL.Types;
using Logitar.Cms.Contracts.Resources;

namespace Logitar.Cms.Schema.Resources
{
  internal class LocaleGraphType : ObjectGraphType<Locale>
  {
    public LocaleGraphType()
    {
      Name = nameof(Locale);
      Description = "Represents a locale supported by this application.";

      Field(x => x.Code).Description("The unique code of the locale.");

      Field(x => x.DisplayName).Description("The display name of the locale.");
    }
  }
}

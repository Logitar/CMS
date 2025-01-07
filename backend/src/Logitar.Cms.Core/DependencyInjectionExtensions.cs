using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Cms.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarCmsCore(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddLogitarIdentityCore()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddTransient<IContentManager, ContentManager>()
      .AddTransient<IContentTypeManager, ContentTypeManager>()
      .AddTransient<IFieldTypeManager, FieldTypeManager>()
      .AddTransient<ILanguageManager, LanguageManager>();
  }
}

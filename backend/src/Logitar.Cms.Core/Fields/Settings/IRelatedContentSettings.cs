using Logitar.Cms.Core.Contents;

namespace Logitar.Cms.Core.Fields.Settings;

public interface IRelatedContentSettings
{
  ContentTypeId ContentTypeId { get; }
  bool IsMultiple { get; }
}

namespace Logitar.Cms.Core.Contents.Models;

public record ContentKey(Guid ContentTypeId, Guid? LanguageId, string UniqueName);

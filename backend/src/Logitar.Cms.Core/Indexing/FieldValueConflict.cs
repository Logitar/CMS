using Logitar.Cms.Core.Contents;

namespace Logitar.Cms.Core.Indexing;

public record FieldValueConflict(Guid FieldId, ContentId ContentId);

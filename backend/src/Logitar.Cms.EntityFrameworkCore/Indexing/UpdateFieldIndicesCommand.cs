using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;

namespace Logitar.Cms.EntityFrameworkCore.Indexing;

internal record UpdateFieldIndicesCommand(ContentLocaleEntity Locale, IReadOnlyDictionary<Guid, string> FieldValues) : INotification;

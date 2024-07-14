using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record ValidateFieldValuesCommand(
  IReadOnlyCollection<FieldValuePayload> Fields,
  bool IsInvariant,
  ContentTypeAggregate ContentType,
  string PropertyName) : IRequest<Unit>; // TODO(fpion): return type

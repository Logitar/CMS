using FluentValidation.Results;
using Logitar.Cms.Contracts.Contents;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record ValidateFieldValuesCommand(
  IReadOnlyCollection<FieldValue> Fields,
  ContentTypeAggregate ContentType,
  ContentAggregate Content,
  LanguageAggregate? Language,
  string PropertyName,
  bool ThrowOnFailure = true) : IRequest<ValidationResult>;

using MediatR;

namespace Logitar.Cms.Core.Languages.Commands;

public record SaveLanguageCommand(LanguageAggregate Language) : IRequest;

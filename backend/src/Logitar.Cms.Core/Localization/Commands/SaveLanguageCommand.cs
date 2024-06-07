using MediatR;

namespace Logitar.Cms.Core.Localization.Commands;

public record SaveLanguageCommand(LanguageAggregate Language) : INotification;

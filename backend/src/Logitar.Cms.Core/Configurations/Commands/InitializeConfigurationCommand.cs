using MediatR;

namespace Logitar.Cms.Core.Configurations.Commands;

public record InitializeConfigurationCommand(string DefaultLocale, string Username, string Password) : IRequest;

using MediatR;

namespace Logitar.Cms.Core.Configurations.Commands;

public record InitializeConfigurationCommand(string Username, string Password) : INotification;

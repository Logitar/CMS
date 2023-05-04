using Logitar.Cms.Contracts.Configurations;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Commands;

internal record InitializeConfiguration(InitializeConfigurationInput Input) : IRequest<Configuration>;

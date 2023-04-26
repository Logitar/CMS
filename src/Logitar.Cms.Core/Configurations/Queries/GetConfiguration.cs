using Logitar.Cms.Contracts.Configurations;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Queries;

internal record GetConfiguration : IRequest<Configuration?>;

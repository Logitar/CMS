using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Queries;

public record ReadSessionQuery(Guid Id) : IRequest<Session>;

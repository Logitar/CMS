using MediatR;

namespace Logitar.Cms.Core.Contents.Commands;

public record SaveContentCommand(ContentAggregate Content) : IRequest;

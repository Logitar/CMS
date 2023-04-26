using Logitar.Cms.Contracts.Sessions;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Commands;

internal record SignIn(SignInInput Input) : IRequest<Session>;

using MediatR;

namespace Logitar.Cms.Core.Users.Events;

public record UserCreated(string Username) : UserSaved, INotification;

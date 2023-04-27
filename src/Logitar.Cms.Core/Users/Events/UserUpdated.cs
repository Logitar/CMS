using MediatR;

namespace Logitar.Cms.Core.Users.Events;

public record UserUpdated : UserSaved, INotification;

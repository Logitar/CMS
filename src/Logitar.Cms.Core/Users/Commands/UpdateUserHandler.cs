using Logitar.Cms.Contracts.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Cms.Core.Users.Commands;

internal class UpdateUserHandler : IRequestHandler<UpdateUser, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserHandler(IApplicationContext applicationContext,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    UpdateUserInput input = request.Input;

    CultureInfo? locale = input.Locale?.GetCultureInfo(nameof(input.Locale));
    Uri? picture = input.Picture?.GetUri(nameof(input.Picture));

    user.Update(_applicationContext.ActorId, input.FirstName, input.LastName, locale, picture);

    if (input.Password != null)
    {
      user.ChangePassword(_applicationContext.Configuration, input.Password, actorId: _applicationContext.ActorId);
    }

    ReadOnlyEmail? email = ReadOnlyEmail.From(input.Email);
    user.SetEmail(email, _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}

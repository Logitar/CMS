using Logitar.Cms.Contracts.Users;
using MediatR;

namespace Logitar.Cms.Core.Users.Commands;

internal class ChangePasswordHandler : IRequestHandler<ChangePassword, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public ChangePasswordHandler(IApplicationContext applicationContext,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(ChangePassword request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    ChangePasswordInput input = request.Input;

    user.ChangePassword(_applicationContext.Configuration, input.Password, input.Current, _applicationContext.ActorId);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}

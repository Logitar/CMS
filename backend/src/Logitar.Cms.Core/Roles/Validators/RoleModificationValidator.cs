using FluentValidation;
using Logitar.Cms.Core.Roles.Models;

namespace Logitar.Cms.Core.Roles.Validators;

internal class RoleModificationValidator : AbstractValidator<RoleModification>
{
  public RoleModificationValidator()
  {
    RuleFor(x => x.Role).NotEmpty();
    RuleFor(x => x.Action).IsInEnum();
  }
}

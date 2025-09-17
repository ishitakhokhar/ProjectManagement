using FluentValidation;
using ProjectManagement.Models;
namespace ProjectManagement.Validator
{
    public class RoleValidator:AbstractValidator<SecRole>
    {
        public RoleValidator()
        {
            RuleFor(role => role.RoleName)
             .NotEmpty().WithMessage("Role Name is required.")
                .MaximumLength(100).WithMessage("Role Name must not exceed 100 characters.");
        }
    }
}

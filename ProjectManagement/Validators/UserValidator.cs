using FluentValidation;
using ProjectManagement.Models; 

namespace ProjectManagement.Validators
{
    public class UserValidator : AbstractValidator<SecUser>
    {
        public UserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("Password hash cannot be empty.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("A valid role must be assigned.");
        }
    }
}
using FluentValidation;
using ProjectManagement.Models;

public class ProjectMemberValidator : AbstractValidator<PrjProjectMember>
{
    public ProjectMemberValidator()
    {
      
        RuleFor(pm => pm.ProjectId)
            .GreaterThan(0).WithMessage("A valid Project ID is required.");

   
        RuleFor(pm => pm.UserId)
            .GreaterThan(0).WithMessage("A valid User ID is required.");

     
        RuleFor(pm => pm.RoleInProject)
            .NotEmpty().WithMessage("Role in project is required.")
            .MaximumLength(50).WithMessage("Role cannot exceed 50 characters.");
    }
}
using FluentValidation;
using ProjectManagement.Models;
namespace ProjectManagement.Validators;
   

    public class ProjectValidator:AbstractValidator<PrjProject>
    {
    public ProjectValidator()
    {
        RuleFor(project=>project.ProjectName)
        .NotEmpty().WithMessage("ProjectName is Required")
        .MaximumLength(100).WithMessage("ProjectName must not exceed 100 characters.");

        RuleFor(project => project.ProjectDescription)
       .NotEmpty().WithMessage("ProjectDescription is Required");

        RuleFor(project => project.ClientName)
       .NotEmpty().WithMessage("ClientName is Required")
       .MaximumLength(100).WithMessage("ClientName must not exceed 100 characters.");

        RuleFor(project => project.ProjectManagerName)
        .NotEmpty().WithMessage("ProjectManagerName is Required")
        .MaximumLength(100).WithMessage("ProjectManagerName must not exceed 100 characters.");
    }
}


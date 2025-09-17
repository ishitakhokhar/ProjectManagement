using FluentValidation;
using ProjectManagement.Models;

public class IssueStatus : AbstractValidator<MstStatus>
{
    public IssueStatus()
    {

        RuleFor(s => s.StatusName)
       .NotEmpty().WithMessage("Status Name is required.")
       .MaximumLength(50).WithMessage("Status Name cannot exceed 50 characters.");
    }
}
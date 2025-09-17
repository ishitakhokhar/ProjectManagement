using FluentValidation;
using ProjectManagement.Models; 
using System;

public class SprintValidator : AbstractValidator<SprSprint>
{
    public SprintValidator()
    {
      
        RuleFor(sprint => sprint.ProjectId)
            .GreaterThan(0).WithMessage("A valid ProjectID is required.");
     
        RuleFor(sprint => sprint.SprintName)
            .NotEmpty().WithMessage("Sprint Name is required.")
            .MaximumLength(100).WithMessage("Sprint Name must not exceed 100 characters.");

        RuleFor(sprint => sprint.StartDate)
            .NotEmpty().WithMessage("Start Date is required.");

        RuleFor(sprint => sprint.EndDate)
            .NotEmpty().WithMessage("End Date is required.")
            .GreaterThan(sprint => sprint.StartDate)
            .WithMessage("End Date must be after the Start Date.");

       
        RuleFor(sprint => sprint.TotalTasks)
            .GreaterThanOrEqualTo(0).WithMessage("Total Tasks cannot be negative.")
            .When(sprint => sprint.TotalTasks.HasValue);
    }
}
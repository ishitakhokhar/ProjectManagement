using FluentValidation;
using ProjectManagement.Models;
using System;
using System.Linq;

public class IssueValidator : AbstractValidator<PrjIssue>
{
    public IssueValidator()
    {
        RuleFor(issue => issue.ProjectId)
            .GreaterThan(0).WithMessage("A valid Project ID is required.");

        RuleFor(issue => issue.StatusId)
            .GreaterThan(0).WithMessage("A valid Status ID is required.");

        RuleFor(issue => issue.CreatedBy)
            .GreaterThan(0).WithMessage("A valid 'Created By' User ID is required.");

        RuleFor(issue => issue.RaisedOn)
            .NotEmpty().WithMessage("Raised On date is required.");

        RuleFor(issue => issue.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(issue => issue.Priority)
            .NotEmpty().WithMessage("Priority is required.")
            .Must(p => new[] { "Low", "Medium", "High", "Critical" }.Contains(p))
            .WithMessage("Priority must be 'Low', 'Medium', 'High', or 'Critical'.");


        RuleFor(issue => issue.AssignedTo)
            .GreaterThan(0).WithMessage("A valid 'Assigned To' User ID is required.")
            .When(issue => issue.AssignedTo.HasValue); 

        RuleFor(issue => issue.Attachment1)
            .MaximumLength(250).WithMessage("Attachment 1 path cannot exceed 250 characters.")
            .When(issue => !string.IsNullOrEmpty(issue.Attachment1));

        RuleFor(issue => issue.Attachment2)
            .MaximumLength(250).WithMessage("Attachment 2 path cannot exceed 250 characters.")
            .When(issue => !string.IsNullOrEmpty(issue.Attachment2));

        RuleFor(issue => issue.DueDate)
            .GreaterThanOrEqualTo(issue => issue.RaisedOn)
            .WithMessage("Due Date must be on or after the date it was raised.")
            .When(issue => issue.DueDate.HasValue); 
    }
}
using FluentValidation;
using ProjectManagement.Models;

public class IssueCommentValidator : AbstractValidator<PrjIssueComment>
{
    public IssueCommentValidator()
    {
        RuleFor(c => c.IssueId)
            .GreaterThan(0).WithMessage("A valid Issue ID is required.");

        RuleFor(c => c.CommentText)
            .NotEmpty().WithMessage("Comment text cannot be empty.");

        RuleFor(c => c.CreatedBy)
            .GreaterThan(0).WithMessage("A valid 'Created By' User ID is required.");

        RuleFor(c => c.CreatedAt)
            .NotEmpty().WithMessage("Creation date is required.");
    }
}
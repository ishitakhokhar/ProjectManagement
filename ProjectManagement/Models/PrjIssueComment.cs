using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ProjectManagement.Models;

public partial class PrjIssueComment
{
    public int CommentId { get; set; }

    public int IssueId { get; set; }

    public string CommentText { get; set; } = null!;

    public bool IsShow { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
    [JsonIgnore]
    public virtual SecUser? CreatedByNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual PrjIssue? Issue { get; set; } = null!;
}


public class ProjectIssueDropDown
{
    public int IssueId { get; set; }
    public string Title { get; set; } = null!;
}
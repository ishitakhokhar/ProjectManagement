using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ProjectManagement.Models;

public partial class PrjProject
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectDescription { get; set; }

    public string? ClientName { get; set; }

    public string? ProjectManagerName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ProjectStatus { get; set; }

    public string? Visibility { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
        
    public DateTime? UpdatedAt { get; set; }
    [JsonIgnore]
    public virtual SecUser? CreatedByNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<PrjIssue>? PrjIssues { get; set; } = new List<PrjIssue>();
    [JsonIgnore]
    public virtual ICollection<PrjProjectMember>? PrjProjectMembers { get; set; } = new List<PrjProjectMember>();
    [JsonIgnore]
    public virtual ICollection<SprSprint>? SprSprints { get; set; } = new List<SprSprint>();
}
